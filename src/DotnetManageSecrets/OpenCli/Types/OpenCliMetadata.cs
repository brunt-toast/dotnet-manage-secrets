using Newtonsoft.Json;

namespace Dev.JoshBrunton.DotnetManageSecrets.OpenCli.Types;

public class OpenCliMetadata
{
    [JsonProperty("name")] public string Name { get; init; } = string.Empty;
    [JsonProperty("value")] public required object Value { get; init; }
}