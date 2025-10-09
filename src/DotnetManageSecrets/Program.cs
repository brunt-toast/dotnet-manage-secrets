using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Dev.JoshBrunton.DotnetManageSecrets;

internal partial class Program
{
    [GeneratedRegex(@"<UserSecretsId>([a-f\d]{8}-[a-f\d]{4}-[a-f\d]{4}-[a-f\d]{4}-[a-f\d]{12})</UserSecretsId>", RegexOptions.Compiled)]
    private static partial Regex UserSecretsIdDeclarationRegex();

    public static void Main(string[] args)
    {
        string editor = GetEditor(args);
        string project = GetProject(args);
        string guid = GetSecretsId(project);

        var secretsFile = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Environment.ExpandEnvironmentVariables(@$"%APPDATA%\Microsoft\UserSecrets\{guid}\secrets.json")
            : Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $".microsoft/usersecrets/{guid}/secrets.json");

        if (!File.Exists(secretsFile))
        {
            File.Create(secretsFile);
        }

        ProcessStartInfo psi = new()
        {
            FileName = editor,
            ArgumentList = { secretsFile }
        };
        using Process? proc = Process.Start(psi);
        if (proc is null)
        {
            Console.Error.WriteLine("The editor process failed to start.");
            Environment.Exit(1);
            return;
        }

        proc.WaitForExit();
    }

    private static string GetProject(string[] args)
    {
        var argList = args.ToList();
        int projectArgIndex = argList.IndexOf("--project");
        if (projectArgIndex == -1)
        {
            return InferProject(Directory.GetCurrentDirectory());
        }

        if (argList.Count < projectArgIndex + 2)
        {
            Console.Error.WriteLine("The --project flag was passed without a value.");
            Environment.Exit(1);
        }

        var givenFile = argList[projectArgIndex + 1];
        if (File.Exists(givenFile))
        {
            return givenFile;
        }

        return InferProject(givenFile);
    }

    private static string GetSecretsId(string project)
    {
        string projectContents = File.ReadAllText(project);
        MatchCollection matches = UserSecretsIdDeclarationRegex().Matches(projectContents);
        if (!matches.Any())
        {
            Console.Error.WriteLine($"Couldn't find the <UserSecretsId> node in {project}");
            Environment.Exit(1);
        }
        if (matches.Count > 1)
        {
            Console.Error.WriteLine($"Too many <UserSecretsId> nodes in {project}. A single node is expected.");
            Environment.Exit(1);
        }

        Group guid = matches[0].Groups[1];
        return guid.Value;
    }

    private static string InferProject(string directory)
    {
        string[] projects = Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories);
        if (projects.Length == 0)
        {
            Console.Error.WriteLine($"No project was found in the directory {directory}. Use the --project option to specify a csproj file or folder.");
            Environment.Exit(1);
        }
        if (projects.Length > 1)
        {
            Console.Error.WriteLine($"No project was found in the directory {directory}. Use the --project option to specify a csproj file or folder.");
            Environment.Exit(1);
        }
        string project = projects[0];
        return project;
    }

    private static string GetEditor(string[] args)
    {
        var argsList = args.ToList();
        int editorIndex = argsList.IndexOf("--editor");
        if (editorIndex != -1)
        {
            if (argsList.Count < editorIndex + 2)
            {
                Console.Error.WriteLine("The --editor flag was passed without a value");
                Environment.Exit(1);
            }

            return argsList[editorIndex + 1];
        }

        var editor = Environment.GetEnvironmentVariable("EDITOR");
        if (string.IsNullOrWhiteSpace(editor))
        {
            Console.Error.WriteLine("The environment variable $EDITOR wasn't set.");
            Environment.Exit(1);
        }

        return editor;
    }
}