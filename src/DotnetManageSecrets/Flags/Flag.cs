using System.CommandLine;

namespace Dev.JoshBrunton.DotnetManageSecrets.Flags;

internal abstract class Flag : Option<bool>
{
    protected Flag(string name, params string[] aliases) : base(name, aliases)
    {
    }
}