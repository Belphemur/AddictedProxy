using InversionOfControl.Model;
using SimpleMvcSitemap;
using SimpleMvcSitemap.Routing;

namespace AddictedProxy.Services.Sitemap.Boostrap;

public class BootstrapSitemapMedia : IBootstrap
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
    {
        services.Configure<SitemapConfig>(configuration.GetSection("Sitemap"));
        services.AddSingleton<ISitemapProvider>(provider => new SitemapProvider(provider.GetRequiredService<IBaseUrlProvider>()));
        services.AddSingleton<IDynamicSitemapIndexProvider, DynamicSitemapIndexProvider>();
        services.AddSingleton<IBaseUrlProvider, BaseUrlProvider>();
    }
}