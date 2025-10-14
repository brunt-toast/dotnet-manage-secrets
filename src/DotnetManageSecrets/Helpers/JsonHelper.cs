using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dev.JoshBrunton.DotnetManageSecrets.Helpers;
internal static class JsonHelper
{
    public static string Clean(string json)
    {
        var rootToken = JToken.Parse(json);
        var dict = rootToken.ToObject<Dictionary<string, object>>() ?? throw new Exception();
        var retToken = new JObject();

        foreach (var kvp in dict)
        {
            string qualifiedKey = kvp.Key.Replace(':', '.').Replace("__", ".");
            AddQualified(retToken, qualifiedKey, kvp.Value);
        }

        return retToken.ToString();
    }

    private static void AddQualified(this JObject obj, string qualifiedKey, object? value)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));
        if (qualifiedKey == null) throw new ArgumentNullException(nameof(qualifiedKey));

        string[] parts = qualifiedKey.Split('.');
        JObject current = obj;

        // Walk through all but the last part, creating intermediate objects as needed
        for (int i = 0; i < parts.Length - 1; i++)
        {
            string part = parts[i];

            if (current[part] is not JObject next)
            {
                next = new JObject();
                current[part] = next;
            }

            current = next;
        }

        string lastPart = parts[^1];
        current[lastPart] = value == null ? JValue.CreateNull() : JToken.FromObject(value);
    }

    public static string Smudge(string json)
    {
        JObject obj = JObject.Parse(json);
        var flat = Flatten(obj);
        var f2 = flat.Select(kvp => new KeyValuePair<string, object>(kvp.Key.Replace('.', ':'), kvp.Value)).ToDictionary();
        return JsonConvert.SerializeObject(f2, Formatting.Indented);
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
                    string path = prefix != null ? $"{prefix}.{property.Name}" : property.Name;
                    FlattenToken(property.Value, result, path);
                }
                break;

            case JArray jArray:
                for (int i = 0; i < jArray.Count; i++)
                {
                    string path = $"{prefix}[{i}]";
                    FlattenToken(jArray[i], result, path);
                }
                break;

            default:
                result[prefix] = (token as JValue)?.Value!;
                break;
        }
    }
}
