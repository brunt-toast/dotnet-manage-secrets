using Dev.JoshBrunton.DotnetManageSecrets.Arguments.ManageSecretsRootCommandArguments;
using Dev.JoshBrunton.DotnetManageSecrets.Consts;
using Dev.JoshBrunton.DotnetManageSecrets.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Extensions.System;
using Dev.JoshBrunton.DotnetManageSecrets.Flags.ManageSecretsRootCommand;
using Dev.JoshBrunton.DotnetManageSecrets.Options.ManageSecretsRootCommandOptions;
using Dev.JoshBrunton.DotnetManageSecrets.Services;
using Dev.JoshBrunton.DotnetManageSecrets.Services.Filters;
using Dev.JoshBrunton.DotnetManageSecrets.Services.GetOutputData;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.CommandLine;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Dev.JoshBrunton.DotnetManageSecrets.Extensions.System.CommandLine;
using Dev.JoshBrunton.DotnetManageSecrets.IO;

namespace Dev.JoshBrunton.DotnetManageSecrets.Commands;

internal class ManageSecretsRootCommand : RootCommand
{
    private const string ConstDescription = """
                                       Manage dotnet user secrets with your editor and format of choice. 
                                       
                                       This program reads the user secrets ID of a given C# project and looks for the associated file in the user secrets folder. It reads the secrets into a sensible format and presents them using the configured editor, then re-formats the edited file into .NET's expected schema before saving. 
                                       
                                       While editing, a copy of the secrets is stored in the system's temp directory. The file is deleted immediately after closing the editor and loading the new values into memory. Note that the file may persist if the program is not allowed to exit gracefully.
                                       
                                       TIP: You can configure default arguments to this command by using the file ~/.config/dotnet-manage-secrets.rsp. 
                                       """;

    private readonly HideValuesFlag _hideValues = new();
    private readonly EscapeWslFlag _escapeWsl = new();
    private readonly ReadonlyFlag _readonly = new();
    private readonly ProjectOption _project = new();
    private readonly EditorOption _editor = new();
    private readonly FormatOption _format = new();
    private readonly LeftoversArgument _leftovers = new();

    public ManageSecretsRootCommand() : base(ConstDescription)
    {
        Options.Add(_hideValues);
        Options.Add(_escapeWsl);
        Options.Add(_readonly);
        Options.Add(_project);
        Options.Add(_editor);
        Options.Add(_format);

        Arguments.Add(_leftovers);

        Subcommands.Add(new OpenCliCommand());

        SetAction(ExecuteAction);
    }

    public void Execute(string[] args)
    {
        string defaultRspPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "dotnet-manage-secrets.rsp");
        if (File.Exists(defaultRspPath))
        {
            args = [.. args, $"@{defaultRspPath}"];
        }

        Parse(args).Invoke();
    }

    private void ExecuteAction(ParseResult parseResult)
    {
        using var _ = ConsoleDiversion.ForParseResult(parseResult);
        parseResult.TerminateIfParseErrors();

        string csProjPath = ProjectLocator.TryGetCsprojPath(parseResult, _project).Unwrap();
        string guid = UserSecretsIdReader.TryGetSecretsId(csProjPath).Unwrap();
        string secretsFolderPath = SecretsFolderLocator.GetFolderForId(guid, parseResult.GetValue(_escapeWsl));
        string secretsFilePath = Path.Join(secretsFolderPath, "secrets.json");

        if (!File.Exists(secretsFilePath))
        {
            Directory.CreateDirectory(secretsFolderPath);
            File.WriteAllText(secretsFilePath, "{}");
        }

        DataFormats format = parseResult.GetValue(_format);
        IFilter filter = FilterFactory.GetFilterForDataFormat(format);

        string jsonFromSecretsFile = File.ReadAllText(secretsFilePath);

        if (parseResult.GetValue(_hideValues))
        {
            jsonFromSecretsFile = ValueObfuscator.Obfuscate(jsonFromSecretsFile);
        }

        string contentForEdit = filter.Clean(jsonFromSecretsFile);

        if (parseResult.GetValue(_readonly))
        {
            Console.WriteLine(contentForEdit);
            Environment.Exit(0);
        }

        IGetOutputData getOutputData;
        if (Console.IsInputRedirected)
        {
            getOutputData = new PipedGetOutputData(contentForEdit);
        }
        else
        {
            string? editor = parseResult.GetValue(_editor);
            if (editor is null)
            {
                Console.Error.WriteLine("No editor could be found via the $EDITOR environment variable or --editor|-e flag, and input was not redirected.");
                Environment.ExitCode = ExitCodes.EditorNotFound;
                return;
            }
            getOutputData = new InteractiveGetOutputData(contentForEdit, editor, parseResult.GetValue(_leftovers) ?? [], format);
        }

        string contentFromEditor = getOutputData.GetOutput();
        if (Environment.ExitCode != 0)
        {
            return;
        }

        string jsonToDump = filter.Smudge(contentFromEditor);

        var inDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonFromSecretsFile) ?? [];
        var outDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonToDump) ?? [];
        if (inDict.Count == outDict.Count && inDict.SequenceEqual(outDict))
        {
            Environment.ExitCode =  ExitCodes.LogicalValueHasNotChanged;
            return;
        }

        File.WriteAllText(secretsFilePath, jsonToDump);
    }
}
