using AddictedProxy.Controllers.Rest;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Sitemap.Logic;
using SitemapCore;
using SitemapCore.Shared;

namespace AddictedProxy.Services.Sitemap;

public class SitemapDynamicMedia : ISitemapDynamicProvider
{
    private readonly ITvShowRepository _tvShowRepository;
    private readonly ILocationHelper _locationHelper;

    public SitemapDynamicMedia(ITvShowRepository tvShowRepository, ILocationHelper locationHelper)
    {
        _tvShowRepository = tvShowRepository;
        _locationHelper = locationHelper;
    }

    public IAsyncEnumerable<SitemapCore.Sitemap> GenerateAsync()
    {
        return _tvShowRepository.GetAllHavingEpisodesAsync()
                                .Select(show =>
                                {
                                    var route = $"/shows/{show.UniqueId}/{show.Name}";
                                    
                                    return new SitemapCore.Sitemap(_locationHelper.GetSitemapUrl(route))
                                    {
                                        Priority = SitemapPriority.AboveNormal,
                                        LastModification = show.LastSeasonRefreshed,
                                        ChangeFrequency = ChangeFrequency.Monthly
                                    };
                                });
    }
}