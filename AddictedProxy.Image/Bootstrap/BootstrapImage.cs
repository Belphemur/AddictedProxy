using AddictedProxy.Image.Logic;
using InversionOfControl.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Providers;


namespace AddictedProxy.Image.Bootstrap;

public class BootstrapImage : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddImageSharp()
                .RemoveProvider<PhysicalFileSystemProvider>()
                .AddProvider<TmdbImageProvider>()
                .SetCache<DistributedImageCache>();
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        app.UseImageSharp();
    }
}