using AddictedProxy.Storage.Store.S3.Bootstrap.EnvVar;
using InversionOfControl.Model;
using InversionOfControl.Service.EnvironmentVariable.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Storage.Store.S3.Bootstrap;

public class BootstrapS3 : IBootstrap, IBootstrapEnvironmentVariable<S3Config, S3ConfigParser>
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddSingleton<IStorageProvider, S3StorageProvider>();
    }

    public EnvVarRegistration<S3Config, S3ConfigParser> EnvVarRegistration { get; } = new("S3_GATEWAY", "S3_ACCESS", "S3_SECRET", "S3_BUCKET");
}