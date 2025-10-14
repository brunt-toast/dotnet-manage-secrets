using System.CommandLine;
using Dev.JoshBrunton.DotnetManageSecrets.Commands;

RootCommand rootCommand = new ManageSecretsRootCommand();
ParseResult parseResult = rootCommand.Parse(args);
parseResult.Invoke();
