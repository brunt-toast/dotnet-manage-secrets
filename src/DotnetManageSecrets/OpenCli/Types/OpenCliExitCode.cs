using Dev.JoshBrunton.DotnetManageSecrets.Enums.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Dev.JoshBrunton.DotnetManageSecrets.OpenCli.Types;

public class OpenCliExitCode
{
    [JsonProperty("code")] public int Code { get; init; }
    [JsonProperty("description")] public string Description { get; init; } = string.Empty;

    public static OpenCliExitCode FromEnum(ExitCodes value)
    {
        var member = value
            .GetType()
            .GetMember(value.ToString())
            .FirstOrDefault();

        DisplayAttribute? displayAttribute = member?.GetCustomAttribute<DisplayAttribute>();
        ObsoleteAttribute? obsoleteAttribute = member?.GetCustomAttribute<ObsoleteAttribute>();

        string description = displayAttribute is null 
            ? value.ToString() 
            : $"{displayAttribute.Name} - {displayAttribute.Description}";

        if (obsoleteAttribute is not null)
        {
            description += $" (OBSOLETE: {obsoleteAttribute.Message})";
        }

        return new OpenCliExitCode
        {
            Code = (int)value,
            Description = description
        };
    }
}