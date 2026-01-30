using System.ComponentModel.DataAnnotations;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;

public enum ErrorCodes
{
    [Display(Name = "Success", Description = "Everything went as planned.")]
    Success = 0,

    [Display(Name = "File not found", Description = "Expected an explicitly named file, but no such file was found.")]
    FileNotFound = 1,
    
    [Display(Name = "Directory not found", Description = "Expected an explicitly named directory, but no such directory was found.")]
    DirectoryNotFound = 2,
    
    [Display(Name = "No matching files", Description = "Searched for a file or files, but no matches were found.")]
    NoMatchingFiles = 3,
    
    [Display(Name = "Project not registered for user secrets", Description = "The given project was not registered for user secrets.")]
    ProjectNotRegisteredForUserSecrets = 4,
    
    [Display(Name = "Factory has no suitable path", Description = "A factory was asked to create a service, but no suitable service was defined for the conditions.")]
    FactoryHasNoSuitablePath = 5,
    
    [Display(Name = "Unknown error", Description = "Heuristically, we weren't supposed to be able to reach this path. Most likely indicative of a bug.")]
    UnknownError = 6,
    
    [Display(Name = "Logical value has not changed", Description = "The logical value has not changed.")]
    LogicalValueHasNotChanged = 7,
}
