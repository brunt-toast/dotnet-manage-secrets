using Dev.JoshBrunton.DotnetManageSecrets.Arguments.ManageSecretsRootCommandArguments;
using Dev.JoshBrunton.DotnetManageSecrets.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Extensions.System;
using Dev.JoshBrunton.DotnetManageSecrets.Extensions.System.CommandLine;
using Dev.JoshBrunton.DotnetManageSecrets.Flags.ManageSecretsRootCommand;
using Dev.JoshBrunton.DotnetManageSecrets.IO;
using Dev.JoshBrunton.DotnetManageSecrets.Options.ManageSecretsRootCommandOptions;
using Dev.JoshBrunton.DotnetManageSecrets.Services;
using Dev.JoshBrunton.DotnetManageSecrets.Services.Filters;
using Dev.JoshBrunton.DotnetManageSecrets.Services.GetOutputData;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.CommandLine;
using System.CommandLine.Help;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Dev.JoshBrunton.DotnetManageSecrets.Enums.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Help;

namespace Dev.JoshBrunton.DotnetManageSecrets.Commands;

internal class ManageSecretsRootCommand : RootCommand
{
    private const string ConstDescription = """
                                       Manage dotnet user secrets with your editor and format of choice. 
                                       
                                       This program reads the user secrets ID of a given .NET project and looks for the associated file in the user secrets folder. It reads the secrets into a sensible format and presents them using the configured editor, then re-formats the edited file into .NET's expected schema before saving. 
                                       
                                       While editing, a copy of the secrets is stored in the system's temp directory. The file is deleted immediately after closing the editor and loading the new values into memory. Note that the file may persist if the program is not allowed to exit gracefully.
                                       
                                       TIP: You can configure default arguments to this command by using the file ~/.config/dotnet-manage-secrets.rsp. 
                                       """;

    private readonly HideValuesFlag _hideValues = new();
    private readonly EscapeWslFlag _escapeWsl = new();
    private readonly ReadonlyFlag _readonly = new();
    private readonly SourceFileOption _sourceFile = new();
    private readonly ProjectOption _project = new();
    private readonly EditorOption _editor = new();
    private readonly FormatOption _format = new();
    private readonly LeftoversArgument _leftovers = new();

    public ManageSecretsRootCommand() : base(ConstDescription)
    {
        Options.Add(_hideValues);
        Options.Add(_escapeWsl);
        Options.Add(_readonly);
        Options.Add(_sourceFile);
        Options.Add(_project);
        Options.Add(_editor);
        Options.Add(_format);

        Arguments.Add(_leftovers);

        Subcommands.Add(new OpenCliCommand());

        foreach (var t in Options)
        {
            if (t is not HelpOption defaultHelpOption)
            {
                continue;
            }

            defaultHelpOption.Action = new HelpActionWithExitCodes((HelpAction)defaultHelpOption.Action!);
            break;
        }

        SetAction(ExecuteAction);
    }

    public void Execute(string[] args)
    {
        if (args.All(x => x != "--no-autorsp"))
        {
            string defaultRspPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "dotnet-manage-secrets.rsp");
            if (File.Exists(defaultRspPath))
            {
                args = [.. args, $"@{defaultRspPath}"];
            }
        }

        Parse(args).Invoke();
    }

    private void ExecuteAction(ParseResult parseResult)
    {
        using var _ = ConsoleDiversion.ForParseResult(parseResult);
        parseResult.TerminateIfParseErrors();

        string secretsFilePath;

        if (parseResult.TryGetValue(_sourceFile, out FileInfo? sourceFile))
        {
            secretsFilePath = sourceFile.FullName;
        }
        else
        {
            string projectPath = ProjectLocator.TryGetProjectPath(parseResult, _project).Unwrap();
            string guid = UserSecretsIdReader.TryGetSecretsId(projectPath).Unwrap();
            string secretsFolderPath = SecretsFolderLocator.GetFolderForId(guid, parseResult.GetValue(_escapeWsl));
            Directory.CreateDirectory(secretsFolderPath);
            secretsFilePath = Path.Join(secretsFolderPath, "secrets.json");
        }

        if (!File.Exists(secretsFilePath))
        {
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

        IGetOutputData getOutputData = IGetOutputData.GetDefault(contentForEdit,
            parseResult.GetValue(_editor),
            parseResult.GetValue(_leftovers) ?? [],
            format).Unwrap();

        string contentFromEditor = getOutputData.GetOutput().Unwrap();

        string jsonToDump = filter.Smudge(contentFromEditor);

        var inDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonFromSecretsFile) ?? [];
        var outDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonToDump) ?? [];
        if (inDict.Count == outDict.Count && inDict.SequenceEqual(outDict))
        {
            Environment.ExitCode = (int)ExitCodes.LogicalValueHasNotChanged;
            return;
        }

        File.WriteAllText(secretsFilePath, jsonToDump);
    }
}
