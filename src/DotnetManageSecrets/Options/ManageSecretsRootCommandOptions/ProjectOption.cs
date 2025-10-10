using System.CommandLine;
using System.CommandLine.Parsing;

namespace Dev.JoshBrunton.DotnetManageSecrets.Options.ManageSecretsRootCommandOptions;
internal class ProjectOption : Option<string>
{
    private string ValueFactory(ArgumentResult _) => Directory.GetCurrentDirectory();

    public ProjectOption() : base("--project", "-p")
    {
        DefaultValueFactory = ValueFactory;
        Description = "A .csproj file, or a directory containing at least one .csproj file in its tree. " +
                      "In the case of multiple csproj files, a selection prompt wil appear. " +
                      "Pass --error-on-multiple-results to prevent this prompt.";
    }
}
