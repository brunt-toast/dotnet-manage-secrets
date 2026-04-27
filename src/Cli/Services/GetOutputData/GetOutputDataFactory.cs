namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Services.GetOutputData;

internal class GetOutputDataFactory : IGetOutputDataFactory
{
    public IGetOutputData GetDataFetcher()
    {
        return Console.IsInputRedirected
            ? new PipedGetOutputData()
            : new EditorGetOutputData();
    }
}

internal interface IGetOutputDataFactory
{
    IGetOutputData GetDataFetcher();
}