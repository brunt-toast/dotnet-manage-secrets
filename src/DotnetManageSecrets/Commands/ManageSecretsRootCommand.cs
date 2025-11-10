using System.Collections.ObjectModel;
using System.CommandLine;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Dev.JoshBrunton.DotnetManageSecrets.Arguments.ManageSecretsRootCommandArguments;
using Dev.JoshBrunton.DotnetManageSecrets.Consts;
using Dev.JoshBrunton.DotnetManageSecrets.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Extensions.System;
using Dev.JoshBrunton.DotnetManageSecrets.Extensions.System.IO;
using Dev.JoshBrunton.DotnetManageSecrets.Options.ManageSecretsRootCommandOptions;
using Dev.JoshBrunton.DotnetManageSecrets.Services;
using Dev.JoshBrunton.DotnetManageSecrets.Services.Filters;

namespace Dev.JoshBrunton.DotnetManageSecrets.Commands;

internal class ManageSecretsRootCommand : RootCommand
{
    private const string ConstDescription = """
                                       Manage dotnet user secrets as JSON with the editor of your choice. 
                                       
                                       This program reads the user secrets ID of a given C# project and looks for the associated file in your user secrets folder. It formats the secrets as nested JSON (rather than the flattened JSON on disk), re-formatting and saving the result once you close your editor. 
                                       
                                       While editing, a copy of the secrets is stored in the system's temp directory. The file is deleted immediately after closing the editor and loading the new values into memory.
                                       
                                       TIP: You can configure default arguments to this command by using the file ~/.config/dotnet-manage-secrets.rsp. 
                                       """;

    private readonly ProjectOption _project = new();
    private readonly EditorOption _editor = new();
    private readonly FormatOption _format = new();
    private readonly LeftoversArgument _leftovers = new();

    public ManageSecretsRootCommand() : base(ConstDescription)
    {
        Options.Add(_project);
        Options.Add(_editor);
        Options.Add(_format);
        Arguments.Add(_leftovers);
        SetAction(ExecuteAction);
    }

    public int Execute(string[] args)
    {
        string defaultRspPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "dotnet-manage-secrets.rsp");
        if (File.Exists(defaultRspPath))
        {
            args = [.. args, $"@{defaultRspPath}"];
        }

        return Parse(args).Invoke();
    }

    private int ExecuteAction(ParseResult parseResult)
    {
        if (parseResult.Errors.Any())
        {
            Console.Error.WriteLine(string.Join(Environment.NewLine, parseResult.Errors.Select(x => x.Message)));
            return ExitCodes.ParseFailure;
        }

        if (parseResult.GetValue(_editor) is not { } editor || string.IsNullOrWhiteSpace(editor))
        {
            Console.Error.WriteLine($"The editor was not found. Please set environment variable $EDITOR or pass {_editor.Name}");
            return ExitCodes.EditorNotFound;
        }

        int gotProject = ProjectLocator.TryGetCsprojPath(parseResult, _project, out string? csprojPath);
        if (gotProject != 0)
        {
            return gotProject;
        }

        if (csprojPath is null)
        {
            Console.Error.WriteLine("Couldn't find a .csproj file");
            return ExitCodes.UnknownError;
        }

        if (!UserSecretsIdReader.TryGetSecretsId(csprojPath, out string? guid))
        {
            Console.Error.WriteLine("Couldn't get a single secrets ID from the chosen project. Expected exactly one <UserSecretsId> node containing a GUID.");
            return ExitCodes.ProjectNotRegisteredForUserSecrets;
        }

        string secretsFilePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Environment.ExpandEnvironmentVariables(@$"%APPDATA%\Microsoft\UserSecrets\{guid}\secrets.json")
            : Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $".microsoft/usersecrets/{guid}/secrets.json");

        if (!File.Exists(secretsFilePath))
        {
            Console.Error.WriteLine($"Project '{csprojPath}' is registered with secrets ID {guid}, but the file '{secretsFilePath}' was not found.");
            Environment.Exit(ExitCodes.SecretsFileNotFound);
        }

        DataFormats format = parseResult.GetValue(_format);
        IFilter filter = format switch
        {
            DataFormats.Json => new JsonFilter(),
            DataFormats.FlatJson => new NoopFilter(),
            DataFormats.Yaml => new YamlFilter(),
            DataFormats.Xml => new XmlFilter(),
            DataFormats.Toml => new TomlFilter(),
            _ => throw new ArgumentOutOfRangeException()
        };

        string dirtyJson = File.ReadAllText(secretsFilePath);
        string cleanJson = filter.Clean(dirtyJson);
        string fileFormat = format switch
        {
            DataFormats.Json or DataFormats.FlatJson => "json",
            DataFormats.Yaml => "yml",
            DataFormats.Xml => "xml",
            DataFormats.Toml => "toml",
            _ => throw new ArgumentOutOfRangeException()
        };
        string targetFileName = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.{fileFormat}");
        FileExtensions.CreateWithoutLease(targetFileName);
        File.WriteAllText(targetFileName, cleanJson);

        ProcessStartInfo psi = new()
        {
            FileName = editor,
        };

        psi.ArgumentList.Add(targetFileName);
        foreach (var arg in parseResult.GetValue(_leftovers) ?? [])
        {
            psi.ArgumentList.Add(arg);
        }

        using Process? proc = Process.Start(psi);
        if (proc is null)
        {
            Console.Error.WriteLine("The editor process failed to start.");
            return ExitCodes.FailedToStartEditor;
        }

        proc.WaitForExit();

        var outJson = File.ReadAllText(targetFileName);
        File.Delete(targetFileName);
        string jsonToDump = filter.Smudge(outJson);
        File.WriteAllText(secretsFilePath, jsonToDump);

        return ExitCodes.Success;
    }
}
