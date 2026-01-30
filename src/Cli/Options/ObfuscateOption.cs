using System.CommandLine;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Options;

internal class ObfuscateOption : Option<bool>
{
    public ObfuscateOption() : base("--obfuscate", "-x")
    {
        Description = "When loading, set all strings, numbers, and booleans to '', 0, and false respectively. " +
                      "Arrays, objects, and nulls remain intact.";
    }
}
