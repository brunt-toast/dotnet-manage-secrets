using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Dev.JoshBrunton.DotnetManageSecrets.Enums.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Options.ManageSecretsRootCommandOptions;

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

        public bool TryGetValue<T>(Option<T> option, [NotNullWhen(true)] out T? value)
        {
            value = parseResult.GetValue(option);
            return value is not null;
        }
    }
}
