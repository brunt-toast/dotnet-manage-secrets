using System.Collections.ObjectModel;
using System.CommandLine;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Dev.JoshBrunton.DotnetManageSecrets.Arguments.ManageSecretsRootCommandArguments;
using Dev.JoshBrunton.DotnetManageSecrets.Consts;
using Dev.JoshBrunton.DotnetManageSecrets.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Extensions.System;
using Dev.JoshBrunton.DotnetManageSecrets.Extensions.System.IO;
using Dev.JoshBrunton.DotnetManageSecrets.Flags.ManageSecretsRootCommand;
using Dev.JoshBrunton.DotnetManageSecrets.Options.ManageSecretsRootCommandOptions;
using Dev.JoshBrunton.DotnetManageSecrets.Services;
using Dev.JoshBrunton.DotnetManageSecrets.Services.Filters;
using Newtonsoft.Json;

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

        int didFindProject = ProjectLocator.TryGetCsprojPath(parseResult, _project, out string? csprojPath);
        if (didFindProject != 0)
        {
            return didFindProject;
        }

        if (csprojPath is null)
        {
            Console.Error.WriteLine($"The task {nameof(ProjectLocator.TryGetCsprojPath)} suggested it succeeded, but its output was null.");
            return ExitCodes.UnknownError;
        }

        if (!UserSecretsIdReader.TryGetSecretsId(csprojPath, out string? guid))
        {
            Console.Error.WriteLine("Couldn't get a single secrets ID from the chosen project. Expected exactly one <UserSecretsId> node containing a GUID.");
            return ExitCodes.ProjectNotRegisteredForUserSecrets;
        }

        string secretsFolderPath;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            secretsFolderPath = Environment.ExpandEnvironmentVariables(@$"%APPDATA%\Microsoft\UserSecrets\{guid}");
        }
        else if (parseResult.GetValue(_escapeWsl))
        {
            ProcessStartInfo cmdPsi = new ProcessStartInfo("cmd.exe")
            {
                ArgumentList = { "/C", "echo", @$"%APPDATA%\Microsoft\UserSecrets\{guid}" },
                RedirectStandardOutput = true
            };
            using var cmdProcess = new Process();
            cmdProcess.StartInfo = cmdPsi;
            cmdProcess.Start();
            cmdProcess.WaitForExit();
            string cmdPath = cmdProcess.StandardOutput.ReadToEnd();

            ProcessStartInfo wslPathPsi = new ProcessStartInfo("wslpath")
            {
                ArgumentList = { "-u", cmdPath },
                RedirectStandardOutput = true
            };
            using var wslPathProcess = new Process();
            wslPathProcess.StartInfo = wslPathPsi;
            wslPathProcess.Start();
            wslPathProcess.WaitForExit();
            secretsFolderPath = wslPathProcess.StandardOutput.ReadToEnd();
        }
        else
        {
            secretsFolderPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                $".microsoft/usersecrets/{guid}");
        }

        string secretsFilePath = Path.Join(secretsFolderPath, "secrets.json");

        if (!File.Exists(secretsFilePath))
        {
            Directory.CreateDirectory(secretsFolderPath);
            File.Create(secretsFilePath, "{}");
        }

        DataFormats format = parseResult.GetValue(_format);
        IFilter filter = format switch
        {
            DataFormats.Json => new JsonFilter(),
            DataFormats.FlatJson => new NoopFilter(),
            DataFormats.Yaml => new YamlFilter(),
            DataFormats.Xml => new XmlFilter(),
            DataFormats.Toml => new TomlFilter(),
            DataFormats.Ini => new IniFilter(),
            _ => throw new ArgumentOutOfRangeException()
        };

        string jsonFromSecretsFile = File.ReadAllText(secretsFilePath);

        if (parseResult.GetValue(_hideValues))
        {
            jsonFromSecretsFile = ValueObfuscator.Obfuscate(jsonFromSecretsFile);
        }

        string contentForEdit = filter.Clean(jsonFromSecretsFile);

        if (parseResult.GetValue(_readonly))
        {
            Console.WriteLine(contentForEdit);
            return 0;
        }

        string fileFormat = format switch
        {
            DataFormats.Json or DataFormats.FlatJson => "json",
            DataFormats.Yaml => "yml",
            DataFormats.Xml => "xml",
            DataFormats.Toml => "toml",
            DataFormats.Ini => "ini",
            _ => throw new ArgumentOutOfRangeException()
        };
        string editingFileName = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.{fileFormat}");
        FileExtensions.Create(editingFileName, contentForEdit);

        ProcessStartInfo psi = new()
        {
            FileName = editor,
        };

        psi.ArgumentList.Add(editingFileName);
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

        var contentFromEditor = File.ReadAllText(editingFileName);
        File.Delete(editingFileName);
        string jsonToDump = filter.Smudge(contentFromEditor);

        var inDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonFromSecretsFile) ?? [];
        var outDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonToDump) ?? [];
        if (inDict.OrderBy(kvp => kvp.Key).SequenceEqual(outDict.OrderBy(kvp => kvp.Key)))
        {
            return ExitCodes.LogicalValueHasNotChanged;
        }

        File.WriteAllText(secretsFilePath, jsonToDump);

        return ExitCodes.Success;
    }
}
