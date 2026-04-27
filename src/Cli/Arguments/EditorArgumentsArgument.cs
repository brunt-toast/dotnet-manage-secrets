using System.CommandLine;
using System.CommandLine.Parsing;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Arguments;

internal class EditorArgumentsArgument : Argument<List<string>>
{
    private static List<string> ValueFactory(ArgumentResult _) => [];

    public EditorArgumentsArgument() : base("editorArgs")
    {
        Description = EditorArgumentsArgumentResources.Description;
        Arity = ArgumentArity.ZeroOrMore;
        DefaultValueFactory = ValueFactory;
    }
}