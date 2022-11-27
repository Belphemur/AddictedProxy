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
}