using System.Diagnostics.CodeAnalysis;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Types;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Services.ProjectLocators;

internal class DirectorySearchProjectLocator : IProjectLocator
{
    private readonly IUserSecretsIdLocator _userSecretsIdLocator;

    public DirectorySearchProjectLocator(IUserSecretsIdLocator userSecretsIdLocator)
    {
        _userSecretsIdLocator = userSecretsIdLocator;
    }

    public Result<string> GetProjectPath(string query)
    {
        if (!Directory.Exists(query))
        {
            return ErrorCodes.DirectoryNotFound;
        }

        if (TryGetProjectFromDirectory(query, out string? projPath))
        {
            return Result<string>.Ok(projPath);
        }

        return Result<string>.Err(ErrorCodes.NoMatchingFiles);
    }

    private bool TryGetProjectFromDirectory(string directory, [NotNullWhen(true)] out string? path)
    {
        path = null;

        string[] projects = Directory.GetFiles(directory, "*.*proj", SearchOption.AllDirectories)
            .Where(x => _userSecretsIdLocator.TryGetSecretsId(x).IsOk)
            .ToArray();

        if (projects.Length == 0)
        {
            return false;
        }

        if (projects.Length == 1)
        {
            path = projects[0];
            return true;
        }

        Console.WriteLine("Multiple viable .*proj files. Did you mean...");
        for (int i = 0; i < projects.Length; i++)
        {
            Console.WriteLine($"[{i + 1}] {projects[i]}");
        }

        do
        {
            Console.WriteLine("Pick an option (default=1): ");
            string? choiceString = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(choiceString))
            {
                path = projects[0];
                return true;
            }

            if (int.TryParse(choiceString, out int choiceInt) && choiceInt > 0 && choiceInt <= projects.Length)
            {
                path = projects[choiceInt - 1];
                return true;
            }

            Console.Error.WriteLine($"[{choiceString}] is not a valid option.");
        } while (true);
    }
}
