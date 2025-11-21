using Dev.JoshBrunton.DotnetManageSecrets.Types;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using Dev.JoshBrunton.DotnetManageSecrets.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Enums.Enums;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services.GetOutputData;

internal interface IGetOutputData
{
    string Input { get; init; }
    Result<string> GetOutput();

    public static Result<IGetOutputData> GetDefault(string contentForEdit, string? editor, IList<string> editorArgs, DataFormats format)
    {
        if (Console.IsInputRedirected)
        {
            return Result<IGetOutputData>.Ok(new PipedGetOutputData(contentForEdit));
        }
        else
        {
            if (editor is null)
            {
                Console.Error.WriteLine("No editor could be found via the $EDITOR environment variable or --editor|-e flag, and input was not redirected.");
                
                return Result<IGetOutputData>.Err(ExitCodes.EditorNotFound);
            }

            return Result<IGetOutputData>.Ok(new InteractiveGetOutputData(contentForEdit, editor, editorArgs, format));
        }
    }
}
