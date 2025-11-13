using System.CommandLine;
using Newtonsoft.Json;

namespace Dev.JoshBrunton.DotnetManageSecrets.OpenCli.Types;

public class OpenCliOption
{
    [JsonProperty("name")] public string Name { get; init; } = string.Empty;
    [JsonProperty("required")] public bool Required { get; init; }
    [JsonProperty("aliases")] public required string[] Aliases { get; init; }
    [JsonProperty("arguments")] public required OpenCliArgument[] Arguments { get; init; }
    [JsonProperty("group")] public string Group { get; init; } = string.Empty;
    [JsonProperty("description")] public string Description { get; init; } = string.Empty;
    [JsonProperty("recursive")] public bool Recursive { get; init; }
    [JsonProperty("hidden")] public bool Hidden { get; init; }
    [JsonProperty("metadata")] public required OpenCliMetadata[] Metadata { get; init; }

    public static OpenCliOption FromSysCommandLineOption(Option option)
    {
        return new OpenCliOption
        {
            Name = option.Name,
            Required = option.Required,
            Aliases = option.Aliases.ToArray(),
            Arguments = [],
            Group = string.Empty,
            Description = option.Description ?? string.Empty,
            Recursive = option.Recursive,
            Hidden = option.Hidden,
            Metadata = []
        };
    }
}