using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dev.JoshBrunton.DotnetManageSecrets.Enums.Enums;

public enum ExitCodes
{
    [Display(Name = "Unknown error", Description = "Something went wrong, but the specific cause couldn't be determined with any certainty. This is probably a result of a bug.")]
    UnknownError = -1,

    [Display(Name = "Success", Description = "Everything went as planned.")]
    Success = 0,

    [Display(Name = "Parse failure", Description = "The engine couldn't parse the command line input.")]
    ParseFailure = 1,

    [Display(Name = "No matching files", Description = "No files matching a given pattern or query were found.")]
    NoMatchingFiles = 2,

    [Display(Name = "Directory not found", Description = "A named directory was not found.")]
    DirectoryNotFound = 3,

    [Display(Name = "Editor not found", Description = "A required editor was not found.")]
    EditorNotFound = 4,

    [Display(Name = "Project not registered for user-secrets", Description = "A project was found, but it had not been registered for user-secrets.")]
    ProjectNotRegisteredForUserSecrets = 5,

    [Display(Name = "Failed to start editor", Description = "The editor failed to start.")]
    FailedToStartEditor = 6,

    [Obsolete("We now create secrets files if they are not found.")]
    [Display(Name = "Secrets file not found", Description = "The user secrets file for the given ID could not be found.")]
    SecretsFileNotFound = 7,

    [Display(Name = "Invalid argument", Description = "An invalid argument was passed to a command.")]
    InvalidArgument = 8,

    [Display(Name = "Logical value unchanged", Description = "The logical value of the data given has not changed, so there's nothing to do.")]
    LogicalValueHasNotChanged = 9,
}
