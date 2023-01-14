using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AddictedProxy.Storage.Store.Compression.Bootstrap;

public class BootstrapStoreCompression : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICompressedStorageProvider, CompressedStorageProvider>();
    }
}