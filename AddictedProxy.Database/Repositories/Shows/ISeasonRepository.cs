#region

using AddictedProxy.Database.Model.Shows;

#endregion

namespace AddictedProxy.Database.Repositories.Shows;

public interface ISeasonRepository
{
    /// <summary>
    ///     Upsert season in the database
    /// </summary>
    Task InsertNewSeasonsAsync(long showId, IEnumerable<Season> seasons, CancellationToken token);

    Task<Season?> GetSeasonForShowAsync(long showId, int seasonNumber, CancellationToken token);

    /// <summary>
    ///     Update the season
    /// </summary>
    Task SaveChangesAsync(CancellationToken token);

    /// <summary>
    /// Get all seasons for the show
    /// </summary>
    /// <param name="showId"></param>
    /// <returns></returns>
    IAsyncEnumerable<Season> GetSeasonsForShowAsync(long showId);

    /// <summary>
    /// Batch-fetch a (TvShowId, SeasonNumber) → SeasonId lookup for a set of show IDs in a single query.
    /// </summary>
    Task<Dictionary<(long TvShowId, int Number), long>> GetSeasonIdLookupAsync(IEnumerable<long> showIds, CancellationToken token);

    /// <summary>
    /// Update the lastRefreshed field of the season
    /// </summary>
    /// <param name="id"></param>
    /// <param name="lastRefreshed"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task UpdateLastRefreshedFromIdAsync(long id, DateTime lastRefreshed, CancellationToken token);

    /// <summary>
    /// Delete seasons for a show that have no episodes and no season packs.
    /// </summary>
    /// <param name="showId">The show to clean up</param>
    /// <param name="token"></param>
    /// <returns>Number of seasons deleted</returns>
    Task<int> DeleteEmptySeasonsForShowAsync(long showId, CancellationToken token);

    /// <summary>
    /// Returns all seasons that have at least one subtitle or season pack subtitle,
    /// as a queryable for sitemap generation.
    /// </summary>
    IQueryable<Season> GetAllForSitemap();
}