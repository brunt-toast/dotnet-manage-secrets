namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Results.MainService;

public sealed record GetContentForUserEditResult
{
    public string Content { get; }
    public string SuggestedFileExtension { get; }
    public string SecretsFilePath { get; }

    public GetContentForUserEditResult(string content, string suggestedFileExtension, string secretsFilePath)
    {
        Content = content;
        SuggestedFileExtension = suggestedFileExtension;
        SecretsFilePath = secretsFilePath;
    }
}
