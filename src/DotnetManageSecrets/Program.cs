using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Dev.JoshBrunton.DotnetManageSecrets;

internal class Program
{
    public static void Main(string[] args)
    {
        ArgsHelper argsHelper = new(args);

        string? editor = Environment.GetEnvironmentVariable("EDITOR");
        if (string.IsNullOrEmpty(editor))
        {
            if (!argsHelper.TryGetValue("--editor", out editor))
            {
                Console.Error.WriteLine("Couldn't determine the editor to use. Please set environment variable $EDITOR or pass --editor.");
                Environment.Exit(1);
            }
        }

        string project = ProjectResolver.GetProject(argsHelper);
        if (!DotnetUserSecretsHelper.TryGetSecretsId(project, out string? guid))
        {
            Console.Error.WriteLine("Couldn't get a single secrets ID from the chosen project. Expected exactly one <UserSecretsId> node containing a GUID.");
            Environment.Exit(1);
        }

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
}