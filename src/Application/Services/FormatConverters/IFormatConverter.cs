using Dev.JoshBrunton.DotnetManageSecrets.Application.Types;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Services.FormatConverters;

public interface IFormatConverter
{
    string SuggestedFileExtension { get; }

    Result<string> Clean(string input);
    Result<string> Smudge(string input);
}