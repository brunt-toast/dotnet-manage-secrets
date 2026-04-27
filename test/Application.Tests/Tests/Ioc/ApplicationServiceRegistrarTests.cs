using Dev.JoshBrunton.DotnetManageSecrets.Application.Ioc;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Tests.Generators.Ioc;
using Microsoft.Extensions.DependencyInjection;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Tests.Tests.Ioc;

[TestClass]
public class ApplicationServiceRegistrarTests
{
    [TestMethod]
    [DynamicData(nameof(ApplicationServiceDefinitionGenerator.GetCliServiceDefinitions), typeof(ApplicationServiceDefinitionGenerator))]
    public void RegisterServices_ShouldCreateRobustContainer(ServiceDescriptor descriptor)
    {
        IServiceCollection sc = new ServiceCollection();
        ApplicationServiceRegistrar.RegisterServices(sc);
        var sp = sc.BuildServiceProvider();
        _ = sp.GetRequiredService(descriptor.ServiceType);
    }
}
