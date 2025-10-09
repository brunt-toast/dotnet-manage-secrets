namespace Dev.JoshBrunton.DotnetManageSecrets.Services.Filters;

public interface IFilter
{
    string Clean(string input);
    string Smudge(string input);
}
