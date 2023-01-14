using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AddictedProxy.Storage.Store.Bootstrap;

public class BootstrapStore : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICompressedStorageProvider, CompressedStorageProvider>();
    }
}