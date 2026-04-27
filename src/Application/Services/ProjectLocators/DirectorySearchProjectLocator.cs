using System.Diagnostics.CodeAnalysis;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Types;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Services.ProjectLocators;

internal class DirectorySearchProjectLocator : IProjectLocator
{
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

    private static bool TryGetProjectFromDirectory(string directory, [NotNullWhen(true)] out string? path)
    {
        path = null;

        // Quick file-content check to filter candidates without spawning subprocesses.
        // Covers both <UserSecretsId> in the project file and package/import references.
        // The actual ID is resolved once by MainService after the project is selected.
        string[] projects = Directory.GetFiles(directory, "*.*proj", SearchOption.AllDirectories)
            .Where(ProjectFileContainsUserSecretsId)
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

    private static bool ProjectFileContainsUserSecretsId(string projectPath)
    {
        try
        {
            return File.ReadAllText(projectPath).Contains("UserSecretsId", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}
