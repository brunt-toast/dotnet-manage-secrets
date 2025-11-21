using System;
using System.Collections.Generic;
using System.Text;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services.GetOutputData;

internal interface IGetOutputData
{
    string Input { get; init; }
    string GetOutput();
}
