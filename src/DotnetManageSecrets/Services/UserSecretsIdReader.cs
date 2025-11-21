using Dev.JoshBrunton.DotnetManageSecrets.Consts;
using Dev.JoshBrunton.DotnetManageSecrets.Types;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services;
internal static partial class UserSecretsIdReader
{
    [GeneratedRegex(@"<UserSecretsId>([A-Fa-f\d]{8}-[A-Fa-f\d]{4}-[A-Fa-f\d]{4}-[A-Fa-f\d]{4}-[A-Fa-f\d]{12})</UserSecretsId>", RegexOptions.Compiled)]
    private static partial Regex UserSecretsIdDeclarationRegex();

    public static Result<string> TryGetSecretsId(string project)
    {
        string projectContents = File.ReadAllText(project);

        MatchCollection matches = UserSecretsIdDeclarationRegex().Matches(projectContents);
        if (matches.Count != 1)
        {
            return Result<string>.Err(ExitCodes.ProjectNotRegisteredForUserSecrets);
        }

        Group guid = matches[0].Groups[1];
        return Result<string>.Ok(guid.Value);
    }
}
