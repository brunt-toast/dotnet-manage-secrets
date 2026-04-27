using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Options;

internal class FormatOption : Option<DataFormats>
{
    private static DataFormats ValueFactory(ArgumentResult _) => 0;

    public FormatOption() : base("--format", "-f")
    {
        DefaultValueFactory = ValueFactory;
        Description = FormatOptionResources.Description;
    }
}
