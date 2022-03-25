using AddictedProxy.Model.Shows;

namespace AddictedProxy.Database.Repositories;

public interface ISeasonRepository
{
    /// <summary>
    ///  Upsert season in the database
    /// </summary>
    Task UpsertSeason(IEnumerable<Season> seasons, CancellationToken token);

    /// <summary>
    /// Get seasons of a show
    /// </summary>
    /// <param name="showId"></param>
    /// <returns></returns>
    IAsyncEnumerable<Season> GetSeasonsForShow(int showId);

    Task<Season?> GetSeasonForShow(int showId, int seasonNumber, CancellationToken token);

    /// <summary>
    /// Update the season
    /// </summary>
    Task UpdateSeasonAsync(Season season, CancellationToken token);
}