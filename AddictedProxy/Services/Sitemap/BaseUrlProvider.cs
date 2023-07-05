using Microsoft.Extensions.Options;
using SimpleMvcSitemap.Routing;

namespace AddictedProxy.Services.Sitemap;

public class BaseUrlProvider : IBaseUrlProvider
{
    public Uri BaseUrl { get; }

    public BaseUrlProvider(IOptions<SitemapConfig> config)
    {
        BaseUrl = new Uri(config.Value.BaseUrl);
    }
}