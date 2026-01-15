using Newtonsoft.Json;

namespace Dev.JoshBrunton.DotnetManageSecrets.LibOpenCli.Types;

public class OpenCliExitCode
{
    [JsonProperty("code")] public int Code { get; init; }
    [JsonProperty("description")] public string Description { get; init; } = string.Empty;

    public static OpenCliExitCode FromKeyValuePair(KeyValuePair<int, string> kvp)
    {
        return new OpenCliExitCode
        {
            Code = kvp.Key,
            Description = kvp.Value
        };
    }
}