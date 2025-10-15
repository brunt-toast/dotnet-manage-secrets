using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.JoshBrunton.DotnetManageSecrets.Arguments.ManageSecretsRootCommandArguments;
internal class LeftoversArgument : Argument<List<string>>
{
    private static List<string> ValueFactory(ArgumentResult _) => [];

    public LeftoversArgument() : base("editorArgs")
    {
        Description = """
                      Unmatched arguments are treated as arguments to be passed to the editor. Any arguments common to this program and the editor may be escaped by adding them after " -- ". 
                      """;
        Arity = ArgumentArity.ZeroOrMore;
        DefaultValueFactory = ValueFactory;
    }
}
