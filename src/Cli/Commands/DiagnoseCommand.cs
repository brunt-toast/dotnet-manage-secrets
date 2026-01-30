using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Cli.Arguments;
using System.CommandLine;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Commands;

internal class DiagnoseCommand : Command
{
    private readonly ErrorCodeArgument _errorCodeArgument;

    public DiagnoseCommand(ErrorCodeArgument errorCodeArgument) : base("diagnose", "Diagnose an error in the program")
    {
        _errorCodeArgument = errorCodeArgument;

        Add(_errorCodeArgument);

        SetAction(ExecuteAction);
    }

    private int ExecuteAction(ParseResult arg)
    {
        var errorCode = (ErrorCodes)arg.GetValue(_errorCodeArgument);

        Console.WriteLine($"{GetDisplayName(errorCode)} ({(int)errorCode})");
        Console.WriteLine();
        Console.WriteLine(GetDescription(errorCode));

        return 0;
    }

    private static string GetDisplayName(Enum source)
    {
        string sourceString = source.ToString();
        return source.GetType()
            .GetMember(sourceString)
            .FirstOrDefault()?
            .GetCustomAttribute<DisplayAttribute>()?
            .GetName() ?? sourceString;
    }

    private static string GetDescription(Enum source)
    {
        string sourceString = source.ToString();
        return source.GetType()
            .GetMember(sourceString)
            .FirstOrDefault()?
            .GetCustomAttribute<DisplayAttribute>()?
            .Description ?? string.Empty;
    }
}
