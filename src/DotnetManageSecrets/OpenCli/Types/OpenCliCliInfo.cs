using Newtonsoft.Json;

namespace Dev.JoshBrunton.DotnetManageSecrets.OpenCli.Types;

public class OpenCliCliInfo
{
    [JsonProperty("title")] public string Title { get; init; } = string.Empty;
    [JsonProperty("summary")] public string Summary { get; init; } = string.Empty;
    [JsonProperty("description")] public string Description { get; init; } = string.Empty;
    [JsonProperty("contact")] public required OpenCliContact Contact { get; init; }
    [JsonProperty("license")] public required OpenCliLicense License { get; init; }
    [JsonProperty("version")] public string Version { get; init; } = string.Empty;
}