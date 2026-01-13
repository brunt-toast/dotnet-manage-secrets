using Dev.JoshBrunton.DotnetManageSecrets.Application.Types;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Services.GetOutputData;

internal class PipedGetOutputData : IGetOutputData
{
    public Result<string> GetOutputData(string _, string _2, string _3, IEnumerable<string> _4)
    {
        return Console.In.ReadToEnd();
    }
}