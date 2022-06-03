using AddictedProxy.Storage.Store.S3.Bootstrap.EnvVar;
using InversionOfControl.Model;
using InversionOfControl.Service.EnvironmentVariable.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AddictedProxy.Storage.Store.S3.Bootstrap;

public class BootstrapS3 : IBootstrapConditional, IBootstrapEnvironmentVariable<S3Config, S3ConfigParser>
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IStorageProvider, S3StorageProvider>();
    }

    public bool ShouldLoadBootstrap(IConfiguration configuration)
    {
        return Environment.GetEnvironmentVariable("STORAGE") == "s3";
    }

    public EnvVarRegistration<S3Config, S3ConfigParser> EnvVarRegistration { get; } = new("S3_GATEWAY", "S3_ACCESS", "S3_SECRET", "S3_BUCKET");
}