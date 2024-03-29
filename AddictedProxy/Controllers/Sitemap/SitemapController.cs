﻿using AddictedProxy.Controllers.Rest;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Sitemap;
using Microsoft.AspNetCore.Mvc;
using SimpleMvcSitemap;

namespace AddictedProxy.Controllers.Sitemap;

[ApiExplorerSettings(IgnoreApi = true)]
public class SitemapController : Controller
{
    private readonly ISitemapProvider _sitemapProvider;
    private readonly IDynamicSitemapIndexProvider _dynamicSitemapIndexProvider;
    private readonly ITvShowRepository _tvShowRepository;

    public SitemapController(ISitemapProvider sitemapProvider, IDynamicSitemapIndexProvider dynamicSitemapIndexProvider, ITvShowRepository tvShowRepository)
    {
        _sitemapProvider = sitemapProvider;
        _dynamicSitemapIndexProvider = dynamicSitemapIndexProvider;
        _tvShowRepository = tvShowRepository;
    }

    [Route("/sitemap/media/{page?}", Name = nameof(Routes.MediaSitemap))]
    [Route("/sitemap.xml")]
    [HttpGet]
    [Produces("text/xml")]
    public async Task<ActionResult> Media([FromRoute] int? page, CancellationToken cancellationToken)
    {
        var shows = _tvShowRepository.GetAllHavingSubtitlesAsync();
        var indexConfiguration = new MediaSitemapIndexConfiguration(shows, page, Url);
        return _dynamicSitemapIndexProvider.CreateSitemapIndex(_sitemapProvider, indexConfiguration);
    }
}