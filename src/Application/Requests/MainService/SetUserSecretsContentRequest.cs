using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Requests.MainService;

public sealed record SetUserSecretsContentRequest
{
    public string SecretsFilePath { get; }
    public string UserContent { get; }
    public DataFormats UserContentFormat { get; }

    public SetUserSecretsContentRequest(string secretsFilePath, string userContent, DataFormats userContentFormat)
    {
        SecretsFilePath = secretsFilePath;
        UserContent = userContent;
        UserContentFormat = userContentFormat;
    }
}
