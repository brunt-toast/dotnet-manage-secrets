using System;
using System.Collections.Generic;
using System.Text;
using Dev.JoshBrunton.DotnetManageSecrets.Cli.Ioc;
using Microsoft.Extensions.DependencyInjection;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Tests.Generators.Ioc;

internal static class CliServiceDefinitionGenerator
{
    public static IEnumerable<object[]> GetCliServiceDefinitions()
    {
        IServiceCollection sc = new ServiceCollection();
        CliServiceRegistrar.RegisterServices(sc);
        foreach (ServiceDescriptor definition in sc)
        {
            yield return [definition];
        }
    }
}
