using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services;
internal static partial class UserSecretsIdReader
{
    [GeneratedRegex(@"<UserSecretsId>([A-Fa-f\d]{8}-[A-Fa-f\d]{4}-[A-Fa-f\d]{4}-[A-Fa-f\d]{4}-[A-Fa-f\d]{12})</UserSecretsId>", RegexOptions.Compiled)]
    private static partial Regex UserSecretsIdDeclarationRegex();

    public static bool TryGetSecretsId(string project, [MaybeNullWhen(false)] out string id)
    {
        string projectContents = File.ReadAllText(project);

        MatchCollection matches = UserSecretsIdDeclarationRegex().Matches(projectContents);
        if (matches.Count != 1)
        {
            id = null;
            return false;
        }

        Group guid = matches[0].Groups[1];
        id = guid.Value;
        return true;
    }
}
