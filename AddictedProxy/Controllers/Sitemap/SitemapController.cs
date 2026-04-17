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
    private readonly ISeasonRepository _seasonRepository;
    private readonly IOptions<SitemapConfig> _sitemapConfig;

    public SitemapController(ISitemapProvider sitemapProvider, IDynamicSitemapIndexProvider dynamicSitemapIndexProvider, ITvShowRepository tvShowRepository, ISeasonRepository seasonRepository, IOptions<SitemapConfig> sitemapConfig)
    {
        _sitemapProvider = sitemapProvider;
        _dynamicSitemapIndexProvider = dynamicSitemapIndexProvider;
        _tvShowRepository = tvShowRepository;
        _seasonRepository = seasonRepository;
        _sitemapConfig = sitemapConfig;
    }

    [Route("/sitemap/media/{page?}", Name = nameof(Routes.MediaSitemap))]
    [Route("/sitemap.xml")]
    [HttpGet]
    [Produces("text/xml")]
    public async Task<ActionResult> Media([FromRoute] int? page, CancellationToken cancellationToken)
    {
        var showItems = _tvShowRepository.GetAllHavingSubtitlesAsync()
            .Select(s => new MediaSitemapItem(s.UniqueId, s.Name, null, s.LastUpdated, s.IsCompleted));

        var seasonItems = _seasonRepository.GetAllForSitemapAsync()
            .Select(s => new MediaSitemapItem(s.TvShow.UniqueId, s.TvShow.Name, s.Number,
                s.LastRefreshed ?? s.TvShow.LastUpdated, false));

        var combined = showItems.Concat(seasonItems);
        var indexConfiguration = new MediaSitemapIndexConfiguration(combined, page, Url, _sitemapConfig);
        return _dynamicSitemapIndexProvider.CreateSitemapIndex(_sitemapProvider, indexConfiguration);
    }
}