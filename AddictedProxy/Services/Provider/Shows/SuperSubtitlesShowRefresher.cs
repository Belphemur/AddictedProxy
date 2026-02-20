using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Services.Provider.Shows;

/// <summary>
/// No-op show refresher for SuperSubtitles.
/// SuperSubtitles data is ingested via dedicated bulk import and incremental update jobs,
/// not through the on-demand refresh pipeline.
/// </summary>
internal class SuperSubtitlesShowRefresher : IProviderShowRefresher
{
    public DataSource Enum => DataSource.SuperSubtitles;

    public bool IsShowNeedsRefresh(TvShow show) => false;

    public Task RefreshShowAsync(TvShow show, ShowExternalId externalId, CancellationToken token) => Task.CompletedTask;
}
