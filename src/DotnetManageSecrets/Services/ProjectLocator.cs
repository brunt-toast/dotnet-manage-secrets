using Dev.JoshBrunton.DotnetManageSecrets.Consts;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services;
internal static class ProjectLocator
{
    public static int TryGetCsprojPath(ParseResult parseResult, Option<string> projectOption, out string? csprojPath)
    {
        csprojPath = null;

        if (parseResult.GetValue(projectOption) is not { } projectPath)
        {
            Console.Error.WriteLine("The project path could not be found.");
            return ExitCodes.UnknownError;
        }

        if (File.Exists(projectPath))
        {
            csprojPath = projectPath;
            return ExitCodes.Success;
        }

        if (Directory.Exists(projectPath))
        {
            if (TryGetCsprojFromDirectory(projectPath, out csprojPath))
            {
                return ExitCodes.Success;
            }

            Console.Error.WriteLine($"Couldn't find any .csproj files with user secrets enabled under directory {projectPath}");
            return ExitCodes.NoMatchingFiles;

        }

        Console.Error.WriteLine($"\"{projectOption.Name}\" is not a file or directory.");
        return ExitCodes.DirectoryNotFound;
    }

    private static bool TryGetCsprojFromDirectory(string directory, out string? path)
    {
        path = null;

        string[] projects = Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories)
        .Where(x => UserSecretsIdReader.TryGetSecretsId(x, out _))
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

            if (int.TryParse(choiceString, out int choiceInt) && choiceInt <= projects.Length)
            {
                path = projects[choiceInt - 1];
                return true;
            }

            Console.Error.WriteLine($"[{choiceString}] is not a valid option.");
        } while (true);
    }
}
