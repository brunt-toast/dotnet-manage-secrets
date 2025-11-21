using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using Dev.JoshBrunton.DotnetManageSecrets.Enums.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Types;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services;

internal static class ProjectLocator
{
    public static Result<string> TryGetCsprojPath(ParseResult parseResult, Option<string> projectOption)
    {
        if (parseResult.GetValue(projectOption) is not { } projectPath)
        {
            Console.Error.WriteLine("The project path could not be found.");
            return Result<string>.Err(ExitCodes.UnknownError);
        }

        if (File.Exists(projectPath))
        {
            return Result<string>.Ok(projectPath);
        }

        if (Directory.Exists(projectPath))
        {
            if (TryGetCsprojFromDirectory(projectPath, out string? csprojPath))
            {
                return Result<string>.Ok(csprojPath);
            }

            Console.Error.WriteLine($"Couldn't find any .csproj files with user secrets enabled under directory {projectPath}");
            return Result<string>.Err(ExitCodes.NoMatchingFiles);

        }

        Console.Error.WriteLine($"\"{projectOption.Name}\" is not a file or directory.");
        return Result<string>.Err(ExitCodes.DirectoryNotFound);
    }

    private static bool TryGetCsprojFromDirectory(string directory, [NotNullWhen(true)] out string? path)
    {
        path = null;

        string[] projects = Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories)
        .Where(x => UserSecretsIdReader.TryGetSecretsId(x).IsOk)
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

        Console.WriteLine("Multiple viable .csproj files. Did you mean...");
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
