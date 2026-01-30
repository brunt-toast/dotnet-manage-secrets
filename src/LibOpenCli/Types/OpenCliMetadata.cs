using Newtonsoft.Json;

namespace Dev.JoshBrunton.DotnetManageSecrets.LibOpenCli.Types;

public class OpenCliMetadata
{
    [JsonProperty("name")] public string Name { get; init; } = string.Empty;
    [JsonProperty("value")] public required object Value { get; init; }

    public static OpenCliMetadata FromKeyValuePair(KeyValuePair<string, object> kvp)
    {
        return new OpenCliMetadata
        {
            Name = kvp.Key,
            Value = kvp.Value,
        };
    }
}