using System.Collections.ObjectModel;
using System.CommandLine;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Dev.JoshBrunton.DotnetManageSecrets.Arguments.ManageSecretsRootCommandArguments;
using Dev.JoshBrunton.DotnetManageSecrets.Helpers;
using Dev.JoshBrunton.DotnetManageSecrets.Options.ManageSecretsRootCommandOptions;

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
    private readonly RawOption _raw = new();
    private readonly LeftoversArgument _leftovers = new();

    public ManageSecretsRootCommand() : base(CharsHelper.TrimLines(ConstDescription))
    {
        Options.Add(_project);
        Options.Add(_editor);
        Options.Add(_raw);
        Arguments.Add(_leftovers);

        SetAction(ExecuteAction);
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

        int gotProject = TryGetCsprojPath(parseResult, out string? csprojPath);
        if (gotProject != 0)
        {
            return gotProject;
        }

        if (csprojPath is null)
        {
            Console.Error.WriteLine("Couldn't find a .csproj file");
            return ExitCodes.UnknownError;
        }

        if (!DotnetUserSecretsHelper.TryGetSecretsId(csprojPath, out string? guid))
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

        bool doRaw = parseResult.GetValue(_raw);

        string targetFileName;
        if (doRaw)
        {
            targetFileName = secretsFilePath;
        }
        else
        {
            string dirtyJson = File.ReadAllText(secretsFilePath);
            string cleanJson = JsonHelper.Clean(dirtyJson);
            targetFileName = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
            File.Create(targetFileName).Dispose();
            File.WriteAllText(targetFileName, cleanJson);
        }

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

        if (!doRaw)
        {
            var outJson = File.ReadAllText(targetFileName);
            File.Delete(targetFileName);
            string jsonToDump = JsonHelper.Smudge(outJson);
            File.WriteAllText(secretsFilePath, jsonToDump);
        }

        return ExitCodes.Success;
    }

    private int TryGetCsprojPath(ParseResult parseResult, out string? csprojPath)
    {
        csprojPath = null;

        if (parseResult.GetValue(_project) is not { } projectPath)
        {
            Console.Error.WriteLine("The project path could not be found.");
            return ExitCodes.UnknownError;
        }

        if (File.Exists(projectPath))
        {
            csprojPath = projectPath;
            return ExitCodes.Success;
        }

        if (Directory.Exists(projectPath))
        {
            if (TryGetCsprojFromDirectory(projectPath, out csprojPath))
            {
                return ExitCodes.Success;
            }

            Console.Error.WriteLine($"Couldn't find any .csproj files with user secrets enabled under directory {projectPath}");
            return ExitCodes.NoMatchingFiles;

        }

        Console.Error.WriteLine($"\"{_project.Name}\" is not a file or directory.");
        return ExitCodes.DirectoryNotFound;
    }

    private bool TryGetCsprojFromDirectory(string directory, out string? path)
    {
        path = null;

        string[] projects = Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories)
        .Where(x => DotnetUserSecretsHelper.TryGetSecretsId(x, out _))
        .ToArray();

        if (projects.Length == 0)
        {
            return false;
        }

        if (projects.Length == 1)
        {
            path = projects[0];
            return true;
        }

        Console.WriteLine("Multiple viable .csproj files. Did you mean...");
        for (int i = 0; i < projects.Length; i++)
        {
            Console.WriteLine($"[{i + 1}] {projects[i]}");
        }

        do
        {
            Console.WriteLine("Pick an option (default=1): ");
            string? choiceString = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(choiceString))
            {
                path = projects[0];
                return true;
            }

            if (int.TryParse(choiceString, out int choiceInt) && choiceInt <= projects.Length)
            {
                path = projects[choiceInt - 1];
                return true;
            }

            Console.Error.WriteLine($"[{choiceString}] is not a valid option.");
        } while (true);
    }
}
