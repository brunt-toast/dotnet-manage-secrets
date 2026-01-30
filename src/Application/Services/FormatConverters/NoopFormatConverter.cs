using Dev.JoshBrunton.DotnetManageSecrets.Application.Types;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Services.FormatConverters;

internal class NoopFormatConverter : IFormatConverter
{
    public string SuggestedFileExtension => "txt";

    public Result<string> Clean(string input)
    {
        return input;
    }

    public Result<string> Smudge(string input)
    {
        return input;
    }
}
