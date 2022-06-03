#region

using AddictedProxy.Storage.Store.Uplink.Bootstrap.EnvVar;
using InversionOfControl.Model;
using InversionOfControl.Service.EnvironmentVariable.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace AddictedProxy.Storage.Store.Uplink.Bootstrap;

public class BootstrapUplink : IBootstrapConditional, IBootstrapEnvironmentVariable<UplinkSettings, UplinkSettingsParser>
{
    public void ConfigureServices(IServiceCollection services,  IConfiguration configuration)
    {
        services.AddSingleton<IStorageProvider, UplinkStorageProvider>();
    }

    public bool ShouldLoadBootstrap(IConfiguration configuration)
    {
        return Environment.GetEnvironmentVariable("STORAGE") == "uplink";
    }

    public EnvVarRegistration<UplinkSettings, UplinkSettingsParser> EnvVarRegistration { get; } = new("UPLINK_ACCESS", "UPLINK_BUCKET");
}