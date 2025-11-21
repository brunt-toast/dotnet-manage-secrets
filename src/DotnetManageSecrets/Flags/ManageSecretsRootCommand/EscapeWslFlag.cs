using System.CommandLine.Parsing;
using System.Runtime.InteropServices;

namespace Dev.JoshBrunton.DotnetManageSecrets.Flags.ManageSecretsRootCommand;

internal class EscapeWslFlag : Flag
{
    public EscapeWslFlag() : base("--escape-wsl", "-w")
    {
        Description = "If running in WSL, modify Windows' user-secrets files, not Linux's. Errors in non-WSL environments.";

        Validators.Add(ValidateLinuxOsPlatform);
        Validators.Add(ValidateWslPathCommand);

        Hidden = !(IsLinux() && CanResolveWslPathBin());
    }

    private static void ValidateWslPathCommand(OptionResult opt)
    {
        if (!CanResolveWslPathBin())
        {
            opt.AddError("Couldn't find /usr/bin/wslpath. Are we really running in WSL?");
        }
    }

    private void ValidateLinuxOsPlatform(OptionResult opt)
    {
        if (!IsLinux())
        {
            opt.AddError($"{Name} can only be used in Linux environments.");
        }
    }

    private static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    private static bool CanResolveWslPathBin() => File.Exists("/usr/bin/wslpath");
}
