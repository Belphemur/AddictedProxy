using AddictedProxy.Sitemap.Logic;
using InversionOfControl.Model;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AddictedProxy.Services.Sitemap.Boostrap;

public class BootstrapSitemapMedia : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddEnumerable(ServiceDescriptor.Scoped<ISitemapDynamicProvider, SitemapDynamicMedia>());
    }
}