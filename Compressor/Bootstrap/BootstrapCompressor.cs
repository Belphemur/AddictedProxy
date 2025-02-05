#region

using Compressor.Factory;
using Compressor.Model;
using InversionOfControl.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#endregion

namespace Compressor.Bootstrap;

public class BootstrapCompressor : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddOptions<CompressorConfig>().Bind(configuration.GetSection(nameof(CompressorConfig)));
        services.AddSingleton<ICompressor, Compressor>();
        services.AddSingleton<CompressorFactory>();
    }
}