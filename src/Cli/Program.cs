using Dev.JoshBrunton.DotnetManageSecrets.Cli.Commands;
using Dev.JoshBrunton.DotnetManageSecrets.Cli.Ioc;
using Microsoft.Extensions.DependencyInjection;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli;

internal class Program
{
    public static int Main(string[] args)
    {
        var sc = new ServiceCollection();
        CliServiceRegistrar.RegisterServices(sc);
        var sp = sc.BuildServiceProvider();

        return sp.GetRequiredService<ManageSecretsRootCommand>().Parse(args).Invoke();
    }
}