using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services.Filters;
internal class YamlFilter : IFilter
{
    private readonly JsonFilter _jsonFilter = new();

    public string Clean(string input)
    {
        string nestedJson = _jsonFilter.Clean(input);
        return JsonToYaml(nestedJson);
    }

    public string Smudge(string input)
    {
        var nestedJson = YamlToJson(input);
        return _jsonFilter.Smudge(nestedJson);
    }

    private static string JsonToYaml(string json)
    {
        var token = JsonConvert.DeserializeObject(json);
        if (token is null)
        {
            throw new ArgumentNullException(json);
        }
        object plainObject = ConvertJTokenToObject(token);

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        return serializer.Serialize(plainObject);
    }

    private static object ConvertJTokenToObject(object token)
    {
        return token switch
        {
            JObject jObject => jObject.ToObject<Dictionary<string, object>>()!
                .ToDictionary(kv => kv.Key, kv => ConvertJTokenToObject(kv.Value)),
            JArray jArray => jArray.Select(ConvertJTokenToObject).ToList(),
            JValue jValue => jValue.Value ?? "",
            _ => token
        };
    }

    private static string YamlToJson(string yaml)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var yamlObject = deserializer.Deserialize<object>(yaml);

        return JsonConvert.SerializeObject(yamlObject, Formatting.Indented);
    }
}
