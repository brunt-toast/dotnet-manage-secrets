namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Results.MainService;

public sealed record GetContentForUserEditResult
{
    public string Content { get; }
    public string SuggestedFileExtension { get; }

    public GetContentForUserEditResult(string content, string suggestedFileExtension)
    {
        Content = content;
        SuggestedFileExtension = suggestedFileExtension;
    }
}
