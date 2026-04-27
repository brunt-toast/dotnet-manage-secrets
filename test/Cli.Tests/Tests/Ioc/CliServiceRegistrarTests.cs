using Dev.JoshBrunton.DotnetManageSecrets.Cli.Ioc;
using Dev.JoshBrunton.DotnetManageSecrets.Cli.Tests.Generators.Ioc;
using Microsoft.Extensions.DependencyInjection;

namespace Dev.JoshBrunton.DotnetManageSecrets.Cli.Tests.Tests.Ioc;

[TestClass]
public class CliServiceRegistrarTests
{
    [TestMethod]
    [DynamicData(nameof(CliServiceDefinitionGenerator.GetCliServiceDefinitions), typeof(CliServiceDefinitionGenerator))]
    public void RegisterServices_ShouldCreateRobustContainer(ServiceDescriptor descriptor)
    {
        IServiceCollection sc = new ServiceCollection();
        CliServiceRegistrar.RegisterServices(sc);
        var sp = sc.BuildServiceProvider();
        _ = sp.GetRequiredService(descriptor.ServiceType);
    }
}
