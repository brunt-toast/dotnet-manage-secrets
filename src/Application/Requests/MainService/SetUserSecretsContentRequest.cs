using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Requests.MainService;

public sealed record SetUserSecretsContentRequest
{
    public string ProjectQuery { get; }
    public string UserContent { get; }
    public DataFormats UserContentFormat { get; }
    public bool EscapeWsl { get; }

    public SetUserSecretsContentRequest(string projectQuery, string userContent, DataFormats userContentFormat, bool escapeWsl)
    {
        ProjectQuery = projectQuery;
        UserContent = userContent;
        UserContentFormat = userContentFormat;
        EscapeWsl = escapeWsl;
    }
}
