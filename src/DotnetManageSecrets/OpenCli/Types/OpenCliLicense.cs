using Newtonsoft.Json;

namespace Dev.JoshBrunton.DotnetManageSecrets.OpenCli.Types;

public class OpenCliLicense
{
    [JsonProperty("name")] public string Name { get; init; } = string.Empty;
    [JsonProperty("identifier")] public string Identifier { get; init; } = string.Empty;
}