using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using Dev.JoshBrunton.DotnetManageSecrets.Arguments.OpenCliCommandArguments;
using Dev.JoshBrunton.DotnetManageSecrets.Consts;
using Dev.JoshBrunton.DotnetManageSecrets.OpenCli;

namespace Dev.JoshBrunton.DotnetManageSecrets.Commands;

internal class OpenCliCommand : Command
{
    private readonly CommandNameArgument _commandNameArg = new();

    public OpenCliCommand() : base("opencli", "Output the OpenCLI description of a command")
    {
        Arguments.Add(_commandNameArg);

        SetAction(ExecuteAction);
    }

    private int ExecuteAction(ParseResult parseResult)
    {
        string? commandName = parseResult.GetValue(_commandNameArg);
        if (commandName is null)
        {
            Console.Error.WriteLine($"The argument \"{_commandNameArg}\" is required.");
            return ExitCodes.InvalidArgument;
        }

        var command = new ManageSecretsRootCommand().Parse(commandName).CommandResult.Command;

        Console.WriteLine(OpenCliParser.GetOpenCliSpec(command,
            licenseName: "MIT",
            licenseIdentifier: "MIT",
            contactName: "Josh Brunton",
            contactEmail: "josh.brunton@proton.me",
            contactUrl: "https://github.com/brunt-toast/dotnet-manage-secrets"));

        return 0;
    }
}
