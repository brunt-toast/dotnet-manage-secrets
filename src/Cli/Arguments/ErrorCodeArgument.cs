using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Arguments;

internal class ErrorCodeArgument : Argument<int>
{
    public ErrorCodeArgument() : base("errorcode")
    {
        Arity = ArgumentArity.ExactlyOne;

        Description = "A non-zero exit code returned by this program.";

        Validators.Add(ValidateErrorCodes);
    }

    private void ValidateErrorCodes(ArgumentResult obj)
    {
        int intCode = obj.GetValue(this);
        ErrorCodes enumCode = Enum.GetValues<ErrorCodes>().FirstOrDefault(x => (int)x == intCode);
        if (enumCode == 0 && intCode != 0)
        {
            obj.AddError($"Exit code {intCode} does not represent a known failure condition. " +
                                    $"Could it have been thrown by the .NET runtime?");
        }
    }
}
