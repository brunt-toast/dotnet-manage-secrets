using Newtonsoft.Json;

namespace Dev.JoshBrunton.DotnetManageSecrets.OpenCli.Types;

public class OpenCliContact
{
    [JsonProperty("name")] public string Name { get; init; } = string.Empty;
    [JsonProperty("url")] public string Url { get; init; } = string.Empty;
    [JsonProperty("email")] public string Email { get; init; } = string.Empty;
}