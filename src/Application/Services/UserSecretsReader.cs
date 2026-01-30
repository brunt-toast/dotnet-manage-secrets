using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Types;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Services;

internal class UserSecretsReader : IUserSecretsReader
{
    public Result<string> ReadUserSecrets(string secretsFile)
    {
        try
        {
            var directory = Path.GetDirectoryName(secretsFile) ?? throw new ArgumentNullException();
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(secretsFile))
            {
                File.WriteAllText(secretsFile, "{}");
            }

            return File.ReadAllText(secretsFile);
        }
        catch (Exception)
        {
            return ErrorCodes.UnknownError;
        }
    }
}

internal interface IUserSecretsReader
{
    Result<string> ReadUserSecrets(string secretsFile);
}