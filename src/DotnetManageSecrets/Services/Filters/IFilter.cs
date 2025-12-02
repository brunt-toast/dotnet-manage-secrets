using Dev.JoshBrunton.DotnetManageSecrets.Types;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services.Filters;

public interface IFilter
{
    Result<string> Clean(string input);
    Result<string> Smudge(string input);
}
