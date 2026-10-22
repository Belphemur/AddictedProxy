using System.Web;
using AddictedProxy.Culture.Extensions;
using AddictedProxy.Controllers.Rest;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Model.Api;
using AddictedProxy.Services.Sitemap;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleMvcSitemap;

namespace AddictedProxy.Controllers.Sitemap;

[ApiExplorerSettings(IgnoreApi = true)]
public class SitemapController : Controller
{
    private readonly ISitemapProvider _sitemapProvider;
    private readonly IDynamicSitemapIndexProvider _dynamicSitemapIndexProvider;
    private readonly ITvShowRepository _tvShowRepository;
    private readonly IOptions<SitemapConfig> _sitemapConfig;

    public SitemapController(ISitemapProvider sitemapProvider, IDynamicSitemapIndexProvider dynamicSitemapIndexProvider, ITvShowRepository tvShowRepository, IOptions<SitemapConfig> sitemapConfig)
    {
        _sitemapProvider = sitemapProvider;
        _dynamicSitemapIndexProvider = dynamicSitemapIndexProvider;
        _tvShowRepository = tvShowRepository;
        _sitemapConfig = sitemapConfig;
    }

    [Route("/sitemap/shows/{page?}", Name = nameof(Routes.ShowsSitemap))]
    [Route("/sitemap/media/{page?}", Name = nameof(Routes.MediaSitemap))]
    [Route("/sitemap.xml")]
    [HttpGet]
    [Produces("text/xml")]
    public ActionResult Media([FromRoute] int? page, CancellationToken cancellationToken)
    {
        var shows = _tvShowRepository.GetAllHavingSubtitlesAsync();
        var indexConfiguration = new MediaSitemapIndexConfiguration(shows, page, Url, _sitemapConfig);
        return _dynamicSitemapIndexProvider.CreateSitemapIndex(_sitemapProvider, indexConfiguration);
    }

    /// <summary>
    /// llms.txt catalog for LLM agents (design: docs/2026-10-21-llm-discoverability.md).
    /// Same data source as the XML sitemap (DRY: one catalog producer) and the same
    /// download-wall invariant — metadata only, never a download URI.
    /// </summary>
    [Route("/llms.txt", Name = nameof(Routes.LlmCatalog))]
    [HttpGet]
    [Produces("text/plain")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 3600)]
    public ContentResult LlmCatalog()
    {
        var content = BuildLlmCatalog(includeDescription: false);
        return Content(content, "text/plain; charset=utf-8");
    }

    [Route("/llms-full.txt", Name = nameof(Routes.LlmFullCatalog))]
    [HttpGet]
    [Produces("text/plain")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 3600)]
    public ContentResult LlmFullCatalog()
    {
        var content = BuildLlmCatalog(includeDescription: true);
        return Content(content, "text/plain; charset=utf-8");
    }

    private string BuildLlmCatalog(bool includeDescription)
    {
        var baseUrl = _sitemapConfig.Value.BaseUrl.TrimEnd('/');
        var builder = new System.Text.StringBuilder();
        builder.AppendLine($"# Subvault");
        builder.AppendLine("> Find all the subtitles in multiple languages like English, French, etc... for your favorite show.");
        builder.AppendLine();
        builder.AppendLine("## Links");
        foreach (var show in _tvShowRepository.GetAllHavingSubtitlesAsync())
        {
            var url = $"{baseUrl}/shows/{Base62Id.Encode(show.UniqueId)}/{HttpUtility.UrlEncode(show.Name.ToSlug())}";
            builder.AppendLine($"- [{show.Name}]({url})");
        }
        return builder.ToString();
    }
}