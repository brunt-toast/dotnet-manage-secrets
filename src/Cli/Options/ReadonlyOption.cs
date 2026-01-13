using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Options;

internal class ReadonlyOption : Option<bool>
{
    public ReadonlyOption() : base("--readonly", "-r")
    {
        Description = "Format the secrets and send them to standard output, then exit; " +
                      "don't launch an editor or accept piped input.";
    }
}
