using System.CommandLine;
using Newtonsoft.Json;

namespace Dev.JoshBrunton.DotnetManageSecrets.OpenCli.Types;

public class OpenCliArgument
{
    [JsonProperty("name")] public string Name { get; init; } = string.Empty;
    [JsonProperty("required")] public bool Required { get; init; }
    [JsonProperty("arity")] public required OpenCliArity Arity { get; init; }
    [JsonProperty("acceptedValues")] public required string[] AcceptedValues { get; init; }
    [JsonProperty("group")] public string Group { get; init; } = string.Empty;
    [JsonProperty("description")] public string Description { get; init; } = string.Empty;
    [JsonProperty("hidden")] public bool Hidden { get; init; }
    [JsonProperty("metadata")] public required OpenCliMetadata[] Metadata { get; init; }

    public static OpenCliArgument FromSysCommandLineArgument(Argument argument)
    {
        return new OpenCliArgument
        {
            Name = argument.Name,
            Required = argument.Arity.MinimumNumberOfValues >= 1,
            Arity = OpenCliArity.FromSysCommandLineArgumentArity(argument.Arity),
            AcceptedValues = [],
            Group = string.Empty,
            Description = argument.Description ?? string.Empty,
            Hidden = argument.Hidden,
            Metadata = []
        };
    }
}