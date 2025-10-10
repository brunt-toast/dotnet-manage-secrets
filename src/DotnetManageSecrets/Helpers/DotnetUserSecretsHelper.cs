using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dev.JoshBrunton.DotnetManageSecrets;
internal static partial class DotnetUserSecretsHelper
{
    [GeneratedRegex(@"<UserSecretsId>([a-f\d]{8}-[a-f\d]{4}-[a-f\d]{4}-[a-f\d]{4}-[a-f\d]{12})</UserSecretsId>", RegexOptions.Compiled)]
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
