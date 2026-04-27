using System.CommandLine;
using System.CommandLine.Parsing;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Localization;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Options;

internal class EscapeWslOption: Option<bool>
{
    private readonly IStringLocalizer<EscapeWslOptionResources> _localizer;
    private const string WslpathPath = "/usr/bin/wslpath";

    public EscapeWslOption(IStringLocalizer<EscapeWslOptionResources> localizer) : base("--escape-wsl", "-w")
    {
        _localizer = localizer;
        Description = EscapeWslOptionResources.Description;

        Validators.Add(ValidateLinuxOsPlatform);
        Validators.Add(ValidateWslPathCommand);

        Hidden = !(IsLinux() && CanResolveWslPathBin());
    }

    private void ValidateWslPathCommand(OptionResult opt)
    {
        if (!opt.Tokens.Any())
        {
            return;
        }

        if (!CanResolveWslPathBin())
        {
            opt.AddError(_localizer[EscapeWslOptionResources.WslpathNotFound, WslpathPath]);
        }
    }

    private void ValidateLinuxOsPlatform(OptionResult opt)
    {
        if (!opt.Tokens.Any())
        {
            return;
        }

        if (!IsLinux())
        {
            opt.AddError(_localizer[EscapeWslOptionResources.OnlyAvailableInLinux, Name]);
        }
    }

    private static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    private static bool CanResolveWslPathBin() => File.Exists(WslpathPath);
}
