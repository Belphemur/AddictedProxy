using AddictedProxy.Storage.Store.Boostrap.EnvVar;
using InversionOfControl.Model;
using InversionOfControl.Service.EnvironmentVariable.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace AddictedProxy.Storage.Store.Boostrap;

public class BootstrapStore : IBootstrap, IBootstrapEnvironmentVariable<UplinkSettings, UplinkSettingsParser>
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IStorageProvider, UplinkStorageProvider>();
    }

    public EnvVarRegistration<UplinkSettings, UplinkSettingsParser> EnvVarRegistration { get; } = new("UPLINK_ACCESS", "UPLINK_BUCKET");
}