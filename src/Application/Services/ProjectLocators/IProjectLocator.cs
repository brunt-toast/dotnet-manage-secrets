using Dev.JoshBrunton.DotnetManageSecrets.Application.Types;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Services.ProjectLocators;

public interface IProjectLocator
{
    Result<string> GetProjectPath(string query);
}
