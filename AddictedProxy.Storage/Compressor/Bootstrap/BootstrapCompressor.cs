using EasyCompressor;
using InversionOfControl.Model;
using Microsoft.Extensions.DependencyInjection;
using ICompressor = AddictedProxy.Storage.Compressor.ICompressor;

namespace AddictedProxy.Storage.Bootstrap;

public class BootstrapCompressor : IBootstrap
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ZstdCompressor>();
        services.AddSingleton<ICompressor, Compressor.ZstdCompressor>();
    }
}