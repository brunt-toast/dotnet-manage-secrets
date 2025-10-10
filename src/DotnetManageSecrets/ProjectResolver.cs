namespace Dev.JoshBrunton.DotnetManageSecrets;

internal static class ProjectResolver
{
    public static string GetProject(ArgsHelper argsHelper)
    {
        if (!argsHelper.TryGetValue("--project", out string? givenFile))
        {
            return InferProject(Directory.GetCurrentDirectory());
        }

        return File.Exists(givenFile) 
            ? givenFile 
            : InferProject(givenFile);
    }

    private static string InferProject(string directory)
    {
        string[] projects = Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories)
            .Where(x => DotnetUserSecretsHelper.TryGetSecretsId(x, out _))
            .ToArray();

        switch (projects.Length)
        {
            case 0:
                Console.Error.WriteLine($"No projects with user secret IDs were found in the directory {directory}. " +
                                        $"Use the --project option to specify a csproj file or folder.");
                Environment.Exit(1);
                return null;
            case > 1:
                return PickValidProject(projects);
            default:
                return projects[0];
        }
    }

    private static string PickValidProject(string[] projects)
    {
        Console.WriteLine("Multiple viable projects:");
        for (int i = 0; i < projects.Length; i++)
        {
            Console.WriteLine($"[{i + 1}] {projects[i]}");
        }
        Console.WriteLine("Choose a number (default=1): ");
        string? choice = Console.ReadLine();

        if (!int.TryParse(choice, out var choiceInt))
        {
            Console.Error.WriteLine($"The choice \"{choice}\" was not a valid number.");
            Environment.Exit(1);
        }

        if (choiceInt > projects.Length || choiceInt <= 0)
        {
            Console.Error.WriteLine($"The choice \"{choiceInt}\" was outside the valid range.");
            Environment.Exit(1);
        }

        return projects[choiceInt - 1];
    }
}
