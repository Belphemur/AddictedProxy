using AddictedProxy.Controllers.Rest;
using AddictedProxy.Database.Repositories.Shows;
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

    [Route("/sitemap/media/{page?}", Name = nameof(Routes.MediaSitemap))]
    [Route("/sitemap.xml")]
    [HttpGet]
    [Produces("text/xml")]
    public async Task<ActionResult> Media([FromRoute] int? page, CancellationToken cancellationToken)
    {
        var shows = _tvShowRepository.GetAllHavingSubtitlesAsync();
        var indexConfiguration = new MediaSitemapIndexConfiguration(shows, page, Url, _sitemapConfig);
        return _dynamicSitemapIndexProvider.CreateSitemapIndex(_sitemapProvider, indexConfiguration);
    }
}