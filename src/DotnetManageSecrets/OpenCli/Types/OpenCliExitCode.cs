using Newtonsoft.Json;

namespace Dev.JoshBrunton.DotnetManageSecrets.OpenCli.Types;

public class OpenCliExitCode
{
    [JsonProperty("code")] public int Code { get; init; }
    [JsonProperty("description")] public string Description { get; init; } = string.Empty;
}