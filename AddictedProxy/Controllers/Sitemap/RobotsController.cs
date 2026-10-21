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
///
/// Crawl policy (download-wall): subtitle files are never offered for crawling —
/// metadata pages stay open for every agent, AI crawlers included. The same
/// policy is enforced by `X-Robots-Tag: noindex, nofollow` on the download
/// endpoint (see SubtitlesController.Download). LLM agents are pointed at
/// /llms.txt, which lists the catalog metadata without any download URI.
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
public class RobotsController : Controller
{
    /// <summary>
    /// AI/LLM crawler user-agents that receive an explicit agent group. They get
    /// the same rules as the wildcard group (catalog open, downloads closed);
    /// the explicit group exists so agents that only honour their own group are
    /// covered even if a wildcard-most-specific rule is applied. One constant,
    /// asserted verbatim by RobotsControllerTests.
    /// </summary>
    public static readonly string[] AiCrawlerUserAgents =
    [
        "GPTBot",
        "OAI-SearchBot",
        "ChatGPT-User",
        "PerplexityBot",
        "ClaudeBot",
        "Claude-Web",
        "Google-Extended",
        "CCBot",
        "Bytespider",
    ];

    private const string DownloadDisallowPath = "/subtitles/download/";

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
        var builder = new System.Text.StringBuilder();
        AppendAgentGroup(builder, ["*"]);
        AppendAgentGroup(builder, AiCrawlerUserAgents);
        builder.AppendLine($"Sitemap: {sitemapUrl}");
        builder.AppendLine("# LLM agent guidance: /llms.txt lists the show catalog (metadata only).");
        return Content(builder.ToString(), "text/plain; charset=utf-8");
    }

    private static void AppendAgentGroup(System.Text.StringBuilder builder, string[] userAgents)
    {
        foreach (var userAgent in userAgents)
        {
            builder.AppendLine($"User-Agent: {userAgent}");
        }
        builder.AppendLine("Allow: /");
        builder.AppendLine($"Disallow: {DownloadDisallowPath}");
        builder.AppendLine();
    }
}
