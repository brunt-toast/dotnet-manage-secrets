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
    }

    private void ValidateWslPathCommand(OptionResult opt)
    {
        if (!File.Exists("/usr/bin/wslpath"))
        {
            opt.AddError("Couldn't find /usr/bin/wslpath. Are we really running in WSL?");
        }
    }

    private void ValidateLinuxOsPlatform(OptionResult opt)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            opt.AddError($"{Name} can only be used in Linux environments.");
        }
    }
}
