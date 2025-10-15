using System.CommandLine;
using Dev.JoshBrunton.DotnetManageSecrets.Commands;

string[] autoRspArgs;
string defaultRspPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "dotnet-manage-secrets.rsp");
if (File.Exists(defaultRspPath))
{
    string content = File.ReadAllText(defaultRspPath);
    autoRspArgs = System.CommandLine.Parsing.CommandLineParser.SplitCommandLine(content).ToArray();
}
else
{
    autoRspArgs = [];
}

RootCommand rootCommand = new ManageSecretsRootCommand();
ParseResult parseResult = rootCommand.Parse([.. args, .. autoRspArgs]);
parseResult.Invoke();
