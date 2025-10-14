using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.JoshBrunton.DotnetManageSecrets.Options.ManageSecretsRootCommandOptions;
internal class RawOption : Option<bool>
{
    private bool ValueFactory(ArgumentResult _) => false;

    public RawOption() : base("raw", "r")
    {
        DefaultValueFactory = ValueFactory;
        Description = "Instead of creating a temporary file and applying a transformation for nested JSON, directly edit the file owned by dotnet.";
    }
}
