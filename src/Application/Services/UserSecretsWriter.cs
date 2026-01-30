namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Services;

internal class UserSecretsWriter : IUserSecretsWriter
{
    public void Write(string filePath, string content)
    {
        File.WriteAllText(filePath, content);
    }
}

internal interface IUserSecretsWriter
{
    void Write(string filePath, string content);
}