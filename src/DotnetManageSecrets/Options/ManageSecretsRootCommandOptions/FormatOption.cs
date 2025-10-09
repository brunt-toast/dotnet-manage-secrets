using Dev.JoshBrunton.DotnetManageSecrets.Enums;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace Dev.JoshBrunton.DotnetManageSecrets.Options.ManageSecretsRootCommandOptions;

internal class FormatOption : Option<DataFormats>
{
    private static DataFormats ValueFactory(ArgumentResult _) => 0;

    public FormatOption() : base("--format", "-f")
    {
        DefaultValueFactory = ValueFactory;
        Description = "The format with which to interact with the file.";
    }
}
