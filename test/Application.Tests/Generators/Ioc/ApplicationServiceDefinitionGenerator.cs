using Dev.JoshBrunton.DotnetManageSecrets.Application.Ioc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Tests.Generators.Ioc;

internal static class ApplicationServiceDefinitionGenerator
{
    public static IEnumerable<object[]> GetCliServiceDefinitions()
    {
        IServiceCollection sc = new ServiceCollection();
        ApplicationServiceRegistrar.RegisterServices(sc);
        foreach (ServiceDescriptor definition in sc)
        {
            yield return [definition];
        }
    }
}
