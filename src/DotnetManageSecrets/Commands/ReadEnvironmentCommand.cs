using Dev.JoshBrunton.DotnetManageSecrets.Enums.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.IO;
using Dev.JoshBrunton.DotnetManageSecrets.OpenCli;
using Dev.JoshBrunton.DotnetManageSecrets.Options.ManageSecretsRootCommandOptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using Dev.JoshBrunton.DotnetManageSecrets.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Services;
using Dev.JoshBrunton.DotnetManageSecrets.Services.Filters;
using Newtonsoft.Json;

namespace Dev.JoshBrunton.DotnetManageSecrets.Commands;

internal class ReadEnvironmentCommand : Command
{
    private readonly FormatOption _format = new();

    public ReadEnvironmentCommand() : base("read-env", "Read the current environment's variables into a given format.")
    {
        Options.Add(_format);
        SetAction(ExecuteAction);
    }

    private int ExecuteAction(ParseResult parseResult)
    {
        using var _ = ConsoleDiversion.ForParseResult(parseResult);

        DataFormats format = parseResult.GetValue(_format);
        IFilter filter = FilterFactory.GetFilterForDataFormat(format);
        IDictionary env = Environment.GetEnvironmentVariables();
        Dictionary<string, string> flatJson = [];
        foreach (DictionaryEntry o in env)
        {
            string key = o.Key.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(key))
            {
                continue;
            }

            flatJson[key] = o.Value?.ToString() ?? string.Empty;
        }

        Console.WriteLine(filter.Clean(JsonConvert.SerializeObject(flatJson)));
        return 0;
    }
}
