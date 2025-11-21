using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services;

internal static class SecretsFolderLocator
{
    public static string GetFolderForId(string guid, bool escapeWsl)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Environment.ExpandEnvironmentVariables(@$"%APPDATA%\Microsoft\UserSecrets\{guid}");
        }
        else if (escapeWsl)
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
            return wslPathProcess.StandardOutput.ReadToEnd();
        }
        else
        {
            return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                $".microsoft/usersecrets/{guid}");
        }
    }
}
