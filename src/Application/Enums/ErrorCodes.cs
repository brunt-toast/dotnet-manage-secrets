namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;

public enum ErrorCodes
{
    FileNotFound = 1,
    DirectoryNotFound = 2,
    NoMatchingFiles = 3,
    ProjectNotRegisteredForUserSecrets = 4,
    FactoryHasNoSuitablePath = 5,
    UnknownError = 6,
}
