using System.CommandLine;
using Newtonsoft.Json;

namespace Dev.JoshBrunton.DotnetManageSecrets.OpenCli.Types;

public class OpenCliCommand
{
    [JsonProperty("name")] public string Name { get; init; } = string.Empty;
    [JsonProperty("aliases")] public required string[] Aliases { get; init; }
    [JsonProperty("options")] public required OpenCliOption[] Options { get; init; }
    [JsonProperty("arguments")] public required OpenCliArgument[] Arguments { get; init; }
    [JsonProperty("commands")] public required OpenCliCommand[] Commands { get; init; }
    [JsonProperty("exitCodes")] public required OpenCliExitCode[] ExitCodes { get; init; }
    [JsonProperty("description")] public string Description { get; init; } = string.Empty;
    [JsonProperty("hidden")] public bool Hidden { get; init; }
    [JsonProperty("examples")] public required string[] Examples { get; init; }
    [JsonProperty("interactive")] public bool Interactive { get; init; }
    [JsonProperty("metadata")] public required OpenCliMetadata[] Metadata { get; init; }

    public static OpenCliCommand FromSysCommandLineCommand(Command command)
    {
        return new OpenCliCommand
        {
            Name = command.Name,
            Aliases = command.Aliases.ToArray(),
            Options = command.Options.Select(OpenCliOption.FromSysCommandLineOption).ToArray(),
            Arguments = command.Arguments.Select(OpenCliArgument.FromSysCommandLineArgument).ToArray(),
            Commands = command.Subcommands.Select(FromSysCommandLineCommand).ToArray(),
            ExitCodes = [],
            Description = command.Description ?? string.Empty,
            Hidden = command.Hidden,
            Examples = [],
            Interactive = false,
            Metadata = []
        };
    }
}