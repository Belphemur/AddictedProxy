using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Database.Repositories.Shows;

public interface ISeasonPackSubtitleRepository
{
    /// <summary>
    /// Get all season pack subtitles for a given show and season
    /// </summary>
    Task<IReadOnlyList<SeasonPackSubtitle>> GetByShowAndSeasonAsync(long tvShowId, int season, CancellationToken token);

    /// <summary>
    /// Get a season pack subtitle by source and external ID
    /// </summary>
    Task<SeasonPackSubtitle?> GetBySourceAndExternalIdAsync(DataSource source, long externalId, CancellationToken token);

    /// <summary>
    /// Add or update multiple season pack subtitles
    /// </summary>
    Task BulkUpsertAsync(IEnumerable<SeasonPackSubtitle> seasonPackSubtitles, CancellationToken token);
}
