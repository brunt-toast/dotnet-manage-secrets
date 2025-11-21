using Dev.JoshBrunton.DotnetManageSecrets.Extensions.System;

namespace Dev.JoshBrunton.DotnetManageSecrets.Services.GetOutputData;

internal class PipedGetOutputData : IGetOutputData
{
    public string Input { get; init; }

    public PipedGetOutputData(string input)
    {
        Input = input;
    }

    public string GetOutput()
    {
        return Console.GetPipedInput();
    }
}