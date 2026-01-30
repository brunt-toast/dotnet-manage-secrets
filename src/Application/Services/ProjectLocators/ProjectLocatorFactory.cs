using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Types;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Services.ProjectLocators;

internal class ProjectLocatorFactory : IProjectLocatorFactory
{
    public Result<IProjectLocator> GetLocator(string query)
    {
        if (File.Exists(query))
        {
            return new FullyQualifiedProjectLocator();
        }

        if (Directory.Exists(query))
        {
            return new DirectorySearchProjectLocator(new UserSecretsIdLocator());
        }

        return ErrorCodes.FactoryHasNoSuitablePath;
    }
}

public interface IProjectLocatorFactory
{
    Result<IProjectLocator> GetLocator(string query);
}