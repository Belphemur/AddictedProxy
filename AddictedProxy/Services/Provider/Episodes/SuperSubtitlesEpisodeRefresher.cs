using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Services.Provider.Episodes;

/// <summary>
/// No-op episode refresher for SuperSubtitles.
/// SuperSubtitles data is ingested via dedicated bulk import and incremental update jobs,
/// not through the on-demand refresh pipeline.
/// </summary>
internal class SuperSubtitlesEpisodeRefresher : IProviderEpisodeRefresher
{
    public DataSource Enum => DataSource.SuperSubtitles;

    public Task<Episode?> GetRefreshEpisodeAsync(ShowExternalId showExternalId, Season season, int episodeNumber, CancellationToken token)
    {
        return Task.FromResult<Episode?>(null);
    }

    public bool IsSeasonNeedRefresh(TvShow show, Season season) => false;
    public Task RefreshEpisodesAsync(ShowExternalId showExternalId, IEnumerable<Season> seasonsToRefresh, Func<int, Task> sendProgress, CancellationToken token)
    {
        return Task.CompletedTask;
    }
}
