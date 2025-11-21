using Dev.JoshBrunton.DotnetManageSecrets.Extensions.System;
using Dev.JoshBrunton.DotnetManageSecrets.Types;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services.GetOutputData;

internal class PipedGetOutputData : IGetOutputData
{
    public string Input { get; init; }

    public PipedGetOutputData(string input)
    {
        Input = input;
    }

    public Result<string> GetOutput()
    {
        return Result<string>.Ok(Console.GetPipedInput());
    }
}