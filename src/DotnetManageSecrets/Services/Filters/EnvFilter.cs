using System.Text;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Dev.JoshBrunton.DotnetManageSecrets.Types;
using Newtonsoft.Json;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services.Filters;

internal partial class EnvFilter : IFilter
{
    [GeneratedRegex(@"([a-zA-Z_]+[a-zA-Z0-9_]*)=(.*)", RegexOptions.Compiled)]
    private static partial Regex EnvLineRegex();

    public Result<string> Clean(string input)
    {
        var rootToken = JToken.Parse(input);
        var dict = rootToken.ToObject<Dictionary<string, object>>() ?? throw new Exception();

        StringBuilder sb = new();
        foreach (var kvp in dict)
        {
            var qualifiedKey = kvp.Key.Replace(":", "__");
            sb.AppendLine($"{qualifiedKey}={kvp.Value}");
        }

        return sb.ToString();
    }

    public Result<string> Smudge(string input)
    {
        Dictionary<string, string> ret = new();
        foreach (string line in input.Split('\n'))
        {
            var match = EnvLineRegex().Match(line);

            if (!match.Success)
            {
                continue;
            }

            string key = match.Groups[1].Value.Replace("__", ":");
            string value = match.Groups[2].Value;

            ret[key] = value;
        }

        return JsonConvert.SerializeObject(ret);
    }
}
