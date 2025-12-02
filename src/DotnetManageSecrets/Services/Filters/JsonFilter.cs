using Dev.JoshBrunton.DotnetManageSecrets.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services.Filters;

internal class JsonFilter : IFilter
{
    public Result<string> Clean(string json)
    {
        var rootToken = JToken.Parse(json);
        var dict = rootToken.ToObject<Dictionary<string, object>>() ?? throw new Exception();
        var retToken = new JObject();

        foreach (var kvp in dict)
        {
            var qualifiedKey = kvp.Key.Replace(':', '.').Replace("__", ".");
            AddQualified(retToken, qualifiedKey, kvp.Value);
        }

        return retToken.ToString();
    }

    public Result<string> Smudge(string json)
    {
        var obj = JObject.Parse(json);
        var flat = Flatten(obj);
        var f2 = flat.Select(kvp => new KeyValuePair<string, object>(kvp.Key.Replace('.', ':'), kvp.Value)).ToDictionary();
        return JsonConvert.SerializeObject(f2, Formatting.Indented);
    }

    private static void AddQualified(JObject obj, string qualifiedKey, object? value)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));
        if (qualifiedKey == null) throw new ArgumentNullException(nameof(qualifiedKey));

        var parts = qualifiedKey.Split('.');
        var current = obj;

        for (var i = 0; i < parts.Length - 1; i++)
        {
            var part = parts[i];

            if (current[part] is not JObject next)
            {
                next = new JObject();
                current[part] = next;
            }

            current = next;
        }

        var lastPart = parts[^1];
        current[lastPart] = value == null ? JValue.CreateNull() : JToken.FromObject(value);
    }

    private static Dictionary<string, object> Flatten(JObject obj)
    {
        var result = new Dictionary<string, object>();
        FlattenToken(obj, result, null!);
        return result;
    }

    private static void FlattenToken(JToken token, Dictionary<string, object> result, string prefix)
    {
        switch (token)
        {
            case JObject jObject:
                foreach (var property in jObject.Properties())
                {
                    var path = prefix != null ? $"{prefix}.{property.Name}" : property.Name;
                    FlattenToken(property.Value, result, path);
                }
                break;

            case JArray jArray:
                for (var i = 0; i < jArray.Count; i++)
                {
                    var path = $"{prefix}[{i}]";
                    FlattenToken(jArray[i], result, path);
                }
                break;

            default:
                result[prefix] = (token as JValue)?.Value!;
                break;
        }
    }
}
