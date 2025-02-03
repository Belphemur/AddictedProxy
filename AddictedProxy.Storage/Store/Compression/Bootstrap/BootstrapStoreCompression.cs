using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Storage.Store.Compression.Bootstrap;

public class BootstrapStoreCompression : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddSingleton<ICompressedStorageProvider, CompressedStorageProvider>();
    }
}