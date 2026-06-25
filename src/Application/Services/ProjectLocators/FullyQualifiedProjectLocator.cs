using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Types;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Services.ProjectLocators;

internal class FullyQualifiedProjectLocator : IProjectLocator
{
    public Result<string> GetProjectPath(string query)
    {
        try
        {
            if (File.Exists(query))
            {
                return query;
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.Error.WriteLine($"Not authorized to access {query}. " +
                                    $"Are you running this from/with the right directory?");
            return ErrorCodes.AccessViolation;
        }

        return ErrorCodes.FileNotFound;
    }
}
