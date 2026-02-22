#region

using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using Microsoft.EntityFrameworkCore;

#endregion

namespace AddictedProxy.Database.Repositories.Shows;

public class SeasonRepository : ISeasonRepository
{
    private readonly EntityContext _entityContext;


    public SeasonRepository(EntityContext entityContext)
    {
        _entityContext = entityContext;
    }

    public async Task InsertNewSeasonsAsync(long showId, IEnumerable<Season> seasons, CancellationToken token)
    {
        var currentSeasons = await _entityContext.Seasons.Where(season => season.TvShowId == showId).AsNoTracking().Select(season => season.Number).ToListAsync(token);
        await _entityContext.Seasons.BulkInsertAsync(seasons.Where(season => !currentSeasons.Contains(season.Number)), token);
    }

    public Task<Season?> GetSeasonForShowAsync(long showId, int seasonNumber, CancellationToken token)
    {
        return _entityContext.Seasons.Where(season => season.TvShow.Id == showId).SingleOrDefaultAsync(season => season.Number == seasonNumber, token);
    }
    
    /// <summary>
    /// Get all seasons for the show
    /// </summary>
    /// <param name="showId"></param>
    /// <returns></returns>
    public IAsyncEnumerable<Season> GetSeasonsForShowAsync(long showId)
    {
        return _entityContext.Seasons.Where(season => season.TvShow.Id == showId).ToAsyncEnumerable();
    }
    
    /// <summary>
    /// Update the lastRefreshed field of the season
    /// </summary>
    /// <param name="id"></param>
    /// <param name="lastRefreshed"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task UpdateLastRefreshedFromIdAsync(long id, DateTime lastRefreshed, CancellationToken token)
    {
        return _entityContext.Seasons.Where(season => season.Id == id).UpdateFromQueryAsync(season => new Season { LastRefreshed = lastRefreshed }, token);
    }

    /// <summary>
    ///     Update the season
    /// </summary>
    public Task SaveChangesAsync(CancellationToken token)
    {
        return _entityContext.SaveChangesAsync(token);
    }
}