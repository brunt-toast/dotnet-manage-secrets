using Newtonsoft.Json;

namespace Dev.JoshBrunton.DotnetManageSecrets.OpenCli.Types;

public class OpenCliConventions
{
    [JsonProperty("true")] public bool GroupOptions { get; init; } = true;
    [JsonProperty("optionArgumentSeparator")] public string OptionArgumentSeparator { get; init; } = " ";}