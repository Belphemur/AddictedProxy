using AddictedProxy.Sitemap.Config;
using AddictedProxy.Sitemap.Logic;
using InversionOfControl.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SitemapCore;

namespace AddictedProxy.Sitemap;

public class BootstrapSitemap : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSitemap(settings =>
        {
            var config = configuration.GetSection("Sitemap").Get<SitemapConfig>();
            settings.BaseUrl = config!.BaseUrl;
            settings.ForUserAgent("*").Allow("*");
            settings.SetDynamicProvider<SitemapDynamicProvider>();
        });
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        app.UseSitemap();
    }
}