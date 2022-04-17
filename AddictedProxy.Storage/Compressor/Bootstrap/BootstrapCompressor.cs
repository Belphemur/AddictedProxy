#region

using AddictedProxy.Storage.Compressor;
using InversionOfControl.Model;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace AddictedProxy.Storage.Bootstrap;

public class BootstrapCompressor : IBootstrap
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ICompressor, BrotliCompressor>();
    }
}