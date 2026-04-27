using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;
using System.CommandLine;
using System.CommandLine.Parsing;
using Microsoft.Extensions.Localization;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Arguments;

internal class ErrorCodeArgument : Argument<int>
{
    private readonly IStringLocalizer<ErrorCodeArgumentResources> _localizer;

    public ErrorCodeArgument(IStringLocalizer<ErrorCodeArgumentResources> localizer) : base("errorcode")
    {
        _localizer = localizer;
        Arity = ArgumentArity.ExactlyOne;

        Description = ErrorCodeArgumentResources.Description;

        Validators.Add(ValidateErrorCodes);
    }

    private void ValidateErrorCodes(ArgumentResult obj)
    {
        int intCode = obj.GetValue(this);
        ErrorCodes enumCode = Enum.GetValues<ErrorCodes>().FirstOrDefault(x => (int)x == intCode);
        if (enumCode == 0 && intCode != 0)
        {
            obj.AddError(_localizer[ErrorCodeArgumentResources.ExitCodeNotKnown, intCode]);
        }
    }
}
