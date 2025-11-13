using Newtonsoft.Json;

namespace Dev.JoshBrunton.DotnetManageSecrets.OpenCli.Types;

public class OpenCliRootObject
{
    [JsonProperty("opencli")] public string OpenCli { get; init; } = string.Empty;
    [JsonProperty("info")] public required OpenCliCliInfo Info { get; init; }
    [JsonProperty("conventions")] public required OpenCliConventions Conventions { get; init; }
    [JsonProperty("arguments")] public required OpenCliArgument[] Arguments { get; init; }
    [JsonProperty("options")] public required OpenCliOption[] Options { get; init; }
    [JsonProperty("commands")] public required OpenCliCommand[] Commands { get; init; }
    [JsonProperty("exitCodes")] public required OpenCliExitCode[] ExitCodes { get; init; }
    [JsonProperty("examples")] public required string[] Examples { get; init; }
    [JsonProperty("interactive")] public bool Interactive { get; init; }
    [JsonProperty("metadata")] public required OpenCliMetadata[] Metadata { get; init; }
}