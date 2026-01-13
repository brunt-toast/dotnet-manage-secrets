using System;
using System.Collections.Generic;
using System.Text;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Types;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Services.GetOutputData;

internal interface IGetOutputData
{
    Result<string> GetOutputData(string inData, string recommendedFileType, string editor, IEnumerable<string> editorArgs);
}
