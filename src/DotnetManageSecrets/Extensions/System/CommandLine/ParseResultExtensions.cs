using Dev.JoshBrunton.DotnetManageSecrets.Consts;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace Dev.JoshBrunton.DotnetManageSecrets.Extensions.System.CommandLine;

internal static class ParseResultExtensions
{
    extension(ParseResult parseResult)
    {
        public void TerminateIfParseErrors()
        {
            if (!parseResult.Errors.Any())
            {
                return;
            }

            Console.Error.WriteLine(string.Join(Environment.NewLine, parseResult.Errors.Select(x => x.Message)));
            Environment.Exit(ExitCodes.ParseFailure);
        }
    }
}
