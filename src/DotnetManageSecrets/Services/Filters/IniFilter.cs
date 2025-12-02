using Newtonsoft.Json.Linq;
using System.Text;
using System.Web;
using Dev.JoshBrunton.DotnetManageSecrets.Types;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services.Filters;

/// <remarks>
///     Adapted from:
///     https://gist.github.com/ritchiecarroll/42909ef62e8597c58ee2301fd2a05e3c
/// </remarks>
internal class IniFilter : IFilter
{
    public Result<string> Clean(string input)
    {
        return JsonToIni(input);
    }

    public Result<string> Smudge(string input)
    {
        return IniToJson(input);
    }

    private static string JsonToIni(string value)
    {
        StringBuilder ini = new();
        JObject json = JObject.Parse(value);
        string eolComment = string.Empty;

        foreach (JProperty property in json.Properties())
        {
            if (property.Value.HasValues)
            {
                ini.AppendLine($"[{property.Name}]{eolComment}");
                eolComment = string.Empty;

                foreach (JToken kvp in property.Value)
                {
                    WriteProperty(kvp as JProperty);
                }
            }
            else
            {
                WriteProperty(property);
            }
        }

        return ini.ToString();

        void WriteProperty(JProperty? property)
        {
            if (property is null)
            {
                return;
            }

            ini.AppendLine($"{property.Name}={property.Value}{eolComment}");
            eolComment = string.Empty;
        }
    }

    private static string IniToJson(string value)
    {
        StringBuilder json = new();
        string[] lines = value.Split(["\r\n", "\n"], StringSplitOptions.None).Select(line => line.Trim()).ToArray();
        string? section = null;
        int kvpIndex = 0;

        json.Append('{');

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";"))
            {
                continue;
            }

            if (line.StartsWith('['))
            {
                if (line.EndsWith(']'))
                {
                    if (section is not null)
                    {
                        json.Append("},");
                    }

                    section = line.Substring(1, line.Length - 2).Trim();
                    json.Append($"\"{HttpUtility.JavaScriptStringEncode(section)}\":{{");
                    kvpIndex = 0;
                }
                else
                {
                    throw new InvalidOperationException($"INI section has an invalid format: \"{line}\"");
                }
            }
            else
            {
                string[] kvp = line.Split('=');

                if (kvp.Length != 2)
                    throw new InvalidOperationException($"INI key-value entry has an invalid format: \"{line}\"");

                json.Append($"{(kvpIndex > 0 ? "," : "")}\"{HttpUtility.JavaScriptStringEncode(kvp[0].Trim())}\":\"{HttpUtility.JavaScriptStringEncode(kvp[1].Trim())}\"");
                kvpIndex++;
            }
        }

        if (section is not null)
        {
            json.Append('}');
        }

        json.Append('}');

        return JToken.Parse(json.ToString()).ToString(Newtonsoft.Json.Formatting.Indented);
    }

}
