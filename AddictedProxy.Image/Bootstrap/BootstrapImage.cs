using AddictedProxy.Image.Logic;
using InversionOfControl.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Providers;


namespace AddictedProxy.Image.Bootstrap;

public class BootstrapImage : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.AddImageSharp()
                .RemoveProvider<PhysicalFileSystemProvider>()
                .AddProvider<TmdbImageProvider>()
                .SetCache<DistributedImageCache>();
        services.AddMemoryCache(options => options.TrackStatistics = true);
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        app.UseImageSharp();
    }
}