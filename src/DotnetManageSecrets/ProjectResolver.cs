using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.JoshBrunton.DotnetManageSecrets;

internal static class ProjectResolver
{
    public static string GetProject(string[] args)
    {
        var argList = args.ToList();
        int projectArgIndex = argList.IndexOf("--project");
        if (projectArgIndex == -1)
        {
            return InferProject(Directory.GetCurrentDirectory());
        }

        if (argList.Count < projectArgIndex + 2)
        {
            Console.Error.WriteLine("The --project flag was passed without a value.");
            Environment.Exit(1);
        }

        var givenFile = argList[projectArgIndex + 1];
        if (File.Exists(givenFile))
        {
            return givenFile;
        }

        return InferProject(givenFile);
    }

    private static string InferProject(string directory)
    {
        string[] projects = Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories);

        if (projects.Length == 0)
        {
            Console.Error.WriteLine($"No project was found in the directory {directory}. Use the --project option to specify a csproj file or folder.");
            Environment.Exit(1);
        }

        if (projects.Length > 1)
        {
            Console.Error.WriteLine($"No project was found in the directory {directory}. Use the --project option to specify a csproj file or folder.");
            Environment.Exit(1);
        }

        string project = projects[0];
        return project;
    }
}
