namespace Dev.JoshBrunton.DotnetManageSecrets.Consts;
internal static class ExitCodes
{
    public const int UnknownError = -1;
    public const int Success = 0;
    public const int ParseFailure = 1;
    public const int NoMatchingFiles = 2;
    public const int DirectoryNotFound = 3;
    public const int EditorNotFound = 4;
    public const int ProjectNotRegisteredForUserSecrets = 5;
    public const int FailedToStartEditor = 6;
    public const int SecretsFileNotFound = 7;
}
