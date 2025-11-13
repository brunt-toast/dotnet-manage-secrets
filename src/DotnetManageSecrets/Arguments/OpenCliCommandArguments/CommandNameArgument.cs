using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Text;

namespace Dev.JoshBrunton.DotnetManageSecrets.Arguments.OpenCliCommandArguments;

internal class CommandNameArgument : Argument<string>
{
    private static string ValueFactory(ArgumentResult arg) => string.Empty;

    public CommandNameArgument() : base("command")
    {
        DefaultValueFactory = ValueFactory;
        Description = "The command to output the specification for. For example, for this command: dotnet manage-secrets opencli opencli";
    }
}
