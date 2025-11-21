using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using Dev.JoshBrunton.DotnetManageSecrets.Enums.Enums;

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
            Environment.Exit((int)ExitCodes.ParseFailure);
        }
    }
}
