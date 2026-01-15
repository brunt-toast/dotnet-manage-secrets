using System.CommandLine;
using Dev.JoshBrunton.DotnetManageSecrets.LibOpenCli.Types;
using Newtonsoft.Json;

namespace Dev.JoshBrunton.DotnetManageSecrets.LibOpenCli.Parser;

public class OpenCliParser
{
    public static string GetOpenCliSpec(Command command,
        Formatting formatting = Formatting.Indented,
        string licenseName = "",
        string licenseIdentifier = "",
        string contactName = "",
        string contactUrl = "",
        string contactEmail = "",
        Dictionary<int, string>? exitCodes = null,
        IEnumerable<string>? examples = null,
        bool isInteractive = false,
        Dictionary<string, object>? metadata = null)
    {
        exitCodes ??= [];
        examples ??= [];
        metadata ??= [];

        OpenCliRootObject obj = Parse(command, licenseName, licenseIdentifier, contactName, contactUrl, contactEmail,
            exitCodes, examples, isInteractive, metadata);
        return JsonConvert.SerializeObject(obj, formatting);
    }

    private static OpenCliRootObject Parse(Command command,
        string licenseName,
        string licenseIdentifier,
        string contactName,
        string contactUrl,
        string contactEmail,
        Dictionary<int, string> exitCodes,
        IEnumerable<string> examples,
        bool isInteractive,
        Dictionary<string, object> metadata)
    {
        var license = new OpenCliLicense
        {
            Name = licenseName,
            Identifier = licenseIdentifier
        };

        var contact = new OpenCliContact
        {
            Name = contactName,
            Url = contactUrl,
            Email = contactEmail
        };

        var info = new OpenCliCliInfo
        {
            Title = command.Name,
            Summary = string.Empty,
            Description = command.Description ?? string.Empty,
            Contact = contact,
            License = license,
            Version = command.GetType().Assembly.GetName().Version?.ToString() ?? "1.0.0"
        };

        return new OpenCliRootObject
        {
            OpenCli = "0.1",
            Info = info,
            Conventions = new OpenCliConventions(),
            Arguments = command.Arguments.Select(OpenCliArgument.FromSysCommandLineArgument).ToArray(),
            Options = command.Options.Select(OpenCliOption.FromSysCommandLineOption).ToArray(),
            Commands = command.Subcommands.Select(OpenCliCommand.FromSysCommandLineCommand).ToArray(),
            ExitCodes = exitCodes.Select(OpenCliExitCode.FromKeyValuePair).ToArray(),
            Examples = examples.ToArray(),
            Interactive = isInteractive,
            Metadata = metadata.Select(OpenCliMetadata.FromKeyValuePair).ToArray()
        };
    }
}
