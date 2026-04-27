using System.CommandLine;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Options;

internal class ReadonlyOption : Option<bool>
{
    public ReadonlyOption() : base("--readonly", "-r")
    {
        Description = ReadonlyOptionResources.Description;
    }
}
