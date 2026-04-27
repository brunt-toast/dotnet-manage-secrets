using System.CommandLine;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Options;

internal class ObfuscateOption : Option<bool>
{
    public ObfuscateOption() : base("--obfuscate", "-x")
    {
        Description = ObfuscateOptionResources.Description;
    }
}
