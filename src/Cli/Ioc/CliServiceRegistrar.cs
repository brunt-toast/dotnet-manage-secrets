using Dev.JoshBrunton.DotnetManageSecrets.Application.Ioc;
using Dev.JoshBrunton.DotnetManageSecrets.Cli.Arguments;
using Dev.JoshBrunton.DotnetManageSecrets.Cli.Commands;
using Dev.JoshBrunton.DotnetManageSecrets.Cli.Options;
using Dev.JoshBrunton.DotnetManageSecrets.Cli.Services.GetOutputData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Ioc;

internal static class CliServiceRegistrar
{
    public static void RegisterServices(IServiceCollection sc)
    {
        ApplicationServiceRegistrar.RegisterServices(sc);

        sc.AddSingleton<ILoggerFactory, NullLoggerFactory>();
        sc.AddLocalization();

        sc.AddSingleton<IGetOutputDataFactory, GetOutputDataFactory>();

        sc.AddTransient<EditorOption>();
        sc.AddTransient<EscapeWslOption>();
        sc.AddTransient<FormatOption>();
        sc.AddTransient<ObfuscateOption>();
        sc.AddTransient<ProjectQueryOption>();
        sc.AddTransient<ReadonlyOption>();

        sc.AddTransient<EditorArgumentsArgument>();
        sc.AddTransient<ErrorCodeArgument>();

        sc.AddTransient<OpenCliCommand>();
        sc.AddTransient<DiagnoseCommand>();
        sc.AddTransient<ManageSecretsCommand>();

        sc.AddTransient<RootDispatcherCommand>();
    }
}
