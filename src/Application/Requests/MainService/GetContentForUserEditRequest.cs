using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Requests.MainService;

public sealed record GetContentForUserEditRequest
{
    public string ProjectQuery { get; }
    public DataFormats DesiredFormat { get; }
    public bool EscapeWsl { get; }
    public bool Obfuscate { get; }

    public GetContentForUserEditRequest(string projectQuery, DataFormats desiredFormat, bool escapeWsl, bool obfuscate)
    {
        ProjectQuery = projectQuery;
        DesiredFormat = desiredFormat;
        EscapeWsl = escapeWsl;
        Obfuscate = obfuscate;
    }
}
