using Dev.JoshBrunton.DotnetManageSecrets.Application.Services;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Services.FormatConverters;
using Dev.JoshBrunton.DotnetManageSecrets.Application.Services.ProjectLocators;
using Microsoft.Extensions.DependencyInjection;

namespace Dev.JoshBrunton.DotnetManageSecrets.Application.Ioc;

public static class ApplicationServiceRegistrar
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IFormatConverterFactory, FormatConverterFactory>();
        services.AddSingleton<IProjectLocatorFactory, ProjectLocatorFactory>();
        services.AddSingleton<IUserSecretsFolderLocator, UserSecretsFolderLocator>();
        services.AddSingleton<IUserSecretsIdLocator, UserSecretsIdLocator>();
        services.AddSingleton<IUserSecretsFolderLocator, UserSecretsFolderLocator>();
        services.AddSingleton<IUserSecretsReader, UserSecretsReader>();
        services.AddSingleton<IUserSecretsWriter, UserSecretsWriter>();
        services.AddSingleton<IValueObfuscator, ValueObfuscator>();

        services.AddSingleton<IMainService, MainService>();
    }
}
