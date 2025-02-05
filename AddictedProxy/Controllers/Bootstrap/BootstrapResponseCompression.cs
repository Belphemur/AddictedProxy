using System.IO.Compression;
using InversionOfControl.Model;
using Microsoft.AspNetCore.ResponseCompression;

namespace AddictedProxy.Controllers.Bootstrap;

public class BootstrapResponseCompression : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Append("text/srt");
        });
        services.AddRequestDecompression();

        services.Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Fastest; });
        services.Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.SmallestSize; });
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        app.UseResponseCompression();
        app.UseRequestDecompression();
    }
}