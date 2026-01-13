using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Types;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Services.ProjectLocators;

internal class FullyQualifiedProjectLocator : IProjectLocator
{
    public Result<string> GetProjectPath(string query)
    {
        if (File.Exists(query))
        {
            return query;
        }

        return ErrorCodes.FileNotFound;
    }
}
