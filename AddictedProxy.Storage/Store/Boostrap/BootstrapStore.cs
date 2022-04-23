#region

using AddictedProxy.Storage.Store.Boostrap.EnvVar;
using InversionOfControl.Model;
using InversionOfControl.Service.EnvironmentVariable.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace AddictedProxy.Storage.Store.Boostrap;

public class BootstrapStore : IBootstrap, IBootstrapEnvironmentVariable<UplinkSettings, UplinkSettingsParser>
{
    public void ConfigureServices(IServiceCollection services,  IConfiguration configuration)
    {
        services.AddSingleton<IStorageProvider, UplinkStorageProvider>();
    }

    public EnvVarRegistration<UplinkSettings, UplinkSettingsParser> EnvVarRegistration { get; } = new("UPLINK_ACCESS", "UPLINK_BUCKET");
}