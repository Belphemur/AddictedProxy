using AddictedProxy.Services.Sitemap;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AddictedProxy.Controllers.Sitemap;

/// <summary>
/// Serves the site robots.txt from configuration so the domain never has to be
/// hardcoded in a static asset. The sitemap URL reuses the same
/// <see cref="SitemapConfig.BaseUrl"/> as the sitemap generation (DRY: one
/// source of truth for the site domain).
///
/// The production Traefik rule routes the www host's robots.txt path here,
/// replacing the Nuxt static `public/robots.txt`.
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
public class RobotsController : Controller
{
    private readonly SitemapConfig _sitemapConfig;

    public RobotsController(IOptions<SitemapConfig> sitemapConfig)
    {
        _sitemapConfig = sitemapConfig.Value;
    }

    [HttpGet]
    [Route("/robots.txt")]
    [Produces("text/plain")]
    public ContentResult Index()
    {
        // BaseUrl has no trailing slash in config; normalize defensively.
        var baseUrl = _sitemapConfig.BaseUrl.TrimEnd('/');
        var sitemapUrl = $"{baseUrl}/sitemap.xml";
        var content =
            $"User-Agent: *\n" +
            $"Allow: /\n" +
            $"\n" +
            $"Sitemap: {sitemapUrl}\n";
        return Content(content, "text/plain; charset=utf-8");
    }
}
