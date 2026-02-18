using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Services.Provider.Seasons;

/// <summary>
/// No-op season refresher for SuperSubtitles.
/// SuperSubtitles data is ingested via dedicated bulk import and incremental update jobs,
/// not through the on-demand refresh pipeline.
/// </summary>
internal class SuperSubtitlesSeasonRefresher : IProviderSeasonRefresher
{
    public DataSource Enum => DataSource.SuperSubtitles;

    public bool IsShowNeedsRefresh(TvShow show) => false;

    public Task<Season?> GetRefreshSeasonAsync(TvShow show, ShowExternalId externalId, int seasonNumber, CancellationToken token)
    {
        return Task.FromResult<Season?>(null);
    }

    public Task RefreshSeasonsAsync(TvShow show, ShowExternalId externalId, CancellationToken token) => Task.CompletedTask;
}
