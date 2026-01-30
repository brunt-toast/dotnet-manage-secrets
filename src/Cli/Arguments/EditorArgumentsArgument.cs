using System.CommandLine;
using System.CommandLine.Parsing;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Arguments;

internal class EditorArgumentsArgument : Argument<List<string>>
{
    private static List<string> ValueFactory(ArgumentResult _) => [];

    public EditorArgumentsArgument() : base("editorArgs")
    {
        Description = """
                      Arguments to pass to the editor. Any arguments common to this program and the editor may be escaped by adding them after " -- ". 
                      """;
        Arity = ArgumentArity.ZeroOrMore;
        DefaultValueFactory = ValueFactory;
    }
}