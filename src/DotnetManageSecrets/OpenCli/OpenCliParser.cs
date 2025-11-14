using System.CommandLine;
using System.Reflection;
using Dev.JoshBrunton.DotnetManageSecrets.OpenCli.Types;
using Newtonsoft.Json;

namespace Dev.JoshBrunton.DotnetManageSecrets.OpenCli;

internal class OpenCliParser
{
    public static string GetOpenCliSpec(Command command, 
        Formatting formatting = Formatting.Indented,
        string licenseName = "",
        string licenseIdentifier = "",
        string contactName = "",
        string contactUrl = "",
        string contactEmail = "")
    {
        return JsonConvert.SerializeObject(Parse(command, licenseName, licenseIdentifier, contactName, contactUrl, contactEmail), formatting);
    }

    private static OpenCliRootObject Parse(Command command, 
        string licenseName = "", 
        string licenseIdentifier = "",
        string contactName = "", 
        string contactUrl = "", 
        string contactEmail = "")
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
            ExitCodes = [],
            Examples = [],
            Interactive = false,
            Metadata = []
        };
    }
}
