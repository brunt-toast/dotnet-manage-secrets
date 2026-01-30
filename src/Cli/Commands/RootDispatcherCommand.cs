using System.CommandLine;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Commands;

internal class RootDispatcherCommand : RootCommand
{
    public RootDispatcherCommand(ManageSecretsCommand manageSecretsCommand,
        DiagnoseCommand diagnoseCommand,
        OpenCliCommand openCliCommand)
    {
        Description = "Core dispatcher command for dotnet-manage-secrets";

        TreatUnmatchedTokensAsErrors = false;

        Add(manageSecretsCommand);
        Add(diagnoseCommand);
        Add(openCliCommand);

        SetAction(async arg =>
        {
            return await manageSecretsCommand.Parse(arg.Tokens.Select(t => t.Value).ToList()).InvokeAsync();
        });
    }
}
