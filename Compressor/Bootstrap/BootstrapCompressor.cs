#region

using Compressor.Factory;
using Compressor.Model;
using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Compressor.Bootstrap;

public class BootstrapCompressor : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CompressorConfig>().Bind(configuration.GetSection(nameof(CompressorConfig)));
        services.AddSingleton<ICompressor, Compressor>();
        services.AddSingleton<CompressorFactory>();
    }
}