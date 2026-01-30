using System.CommandLine;
using Newtonsoft.Json;

namespace Dev.JoshBrunton.DotnetManageSecrets.LibOpenCli.Types;

public class OpenCliArity
{
    [JsonProperty("minimum")] public int Minimum { get; init; }
    [JsonProperty("maximum")] public int Maximum { get; init; }

    public static OpenCliArity FromSysCommandLineArgumentArity(ArgumentArity argumentArity)
    {
        return new OpenCliArity
        {
            Minimum = argumentArity.MinimumNumberOfValues,
            Maximum = argumentArity.MaximumNumberOfValues
        };
    }
}