using System.CommandLine;

namespace Dev.JoshBrunton.DotnetManageSecrets.Options.ManageSecretsRootCommandOptions;

internal class SourceFileOption : Option<FileInfo>
{
    public SourceFileOption() : base("--source-file", "-s")
    {
        Description = "Instead of looking up a user-secrets ID from an SDK-style project, interact directly with a file on disk.";
    }
}
