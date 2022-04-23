#region

using AddictedProxy.Storage.Compressor;
using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace AddictedProxy.Storage.Bootstrap;

public class BootstrapCompressor : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICompressor, BrotliCompressor>();
    }
}