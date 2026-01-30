using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Enums;
using Dev.JoshBrunton.DotnetManageSecrets.LibOpenCli.Parser;
using Microsoft.Extensions.DependencyInjection;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Commands;

internal class OpenCliCommand : Command
{
    private readonly IServiceProvider _serviceProvider;

    public OpenCliCommand(IServiceProvider serviceProvider) : 
        base("opencli", "Gather information about this command in the OpenCLI format.")
    {
        _serviceProvider = serviceProvider;

        SetAction(ExecuteAction);
    }

    private int ExecuteAction(ParseResult _)
    {
        var command = _serviceProvider.GetRequiredService<RootDispatcherCommand>()
            .Parse([]).CommandResult.Command;

        Console.WriteLine(OpenCliParser.GetOpenCliSpec(command,
            licenseName: "MIT",
            licenseIdentifier: "MIT",
            contactName: "Josh Brunton",
            contactEmail: "josh.brunton@proton.me",
            contactUrl: "https://github.com/brunt-toast/dotnet-manage-secrets",
            exitCodes: Enum.GetValues<ErrorCodes>().Select(x => new KeyValuePair<int,string>((int)x, string.Empty)).ToDictionary(),
            isInteractive: true));

        return 0;
    }
}
