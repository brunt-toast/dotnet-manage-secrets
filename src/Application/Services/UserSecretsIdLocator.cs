using Dev.JoshBrunton.DotnetManageSecrets.Application.Types;
using System.Text.RegularExpressions;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Services;

internal partial class UserSecretsIdLocator : IUserSecretsIdLocator
{
    [GeneratedRegex(@"<UserSecretsId>([A-Fa-f\d]{8}-[A-Fa-f\d]{4}-[A-Fa-f\d]{4}-[A-Fa-f\d]{4}-[A-Fa-f\d]{12})</UserSecretsId>", 
        RegexOptions.Compiled)]
    private static partial Regex UserSecretsIdDeclarationRegex();

    public Result<string> TryGetSecretsId(string projectPath)
    {
        string projectContents = File.ReadAllText(projectPath);

        MatchCollection matches = UserSecretsIdDeclarationRegex().Matches(projectContents);
        if (matches.Count != 1)
        {
            return Result<string>.Err(ErrorCodes.ProjectNotRegisteredForUserSecrets);
        }

        Group guid = matches[0].Groups[1];
        return Result<string>.Ok(guid.Value);
    }
}

internal interface IUserSecretsIdLocator
{
    Result<string> TryGetSecretsId(string projectPath);
}