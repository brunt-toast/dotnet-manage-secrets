using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Services;

internal class UserSecretsFolderLocator : IUserSecretsFolderLocator
{
    public string GetFolderForId(string guid, bool escapeWsl)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Environment.ExpandEnvironmentVariables(@$"%APPDATA%\Microsoft\UserSecrets\{guid}");
        }

        if (escapeWsl)
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
            var ret = wslPathProcess.StandardOutput.ReadToEnd();
            ret = ret.Replace("\r", "").Replace("\n", "");
            return ret;
        }

        return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),$".microsoft/usersecrets/{guid}");
    }
}

internal interface IUserSecretsFolderLocator
{
    string GetFolderForId(string guid, bool escapeWsl);
}