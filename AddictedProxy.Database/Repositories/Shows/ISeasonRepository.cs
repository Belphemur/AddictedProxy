#region

using AddictedProxy.Database.Model.Shows;

#endregion

namespace AddictedProxy.Database.Repositories.Shows;

public interface ISeasonRepository
{
    /// <summary>
    ///     Upsert season in the database
    /// </summary>
    Task UpsertSeason(IEnumerable<Season> seasons, CancellationToken token);

    /// <summary>
    ///     Get seasons of a show
    /// </summary>
    /// <param name="showId"></param>
    /// <returns></returns>
    IAsyncEnumerable<Season> GetSeasonsForShow(int showId);

    Task<Season?> GetSeasonForShow(long showId, int seasonNumber, CancellationToken token);

    /// <summary>
    ///     Update the season
    /// </summary>
    Task SaveChangesAsync(CancellationToken token);
}