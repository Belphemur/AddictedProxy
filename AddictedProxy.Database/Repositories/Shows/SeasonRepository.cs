#region

using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using Microsoft.EntityFrameworkCore;
using System.Collections.Frozen;

#endregion

namespace AddictedProxy.Database.Repositories.Shows;

public class SeasonRepository : ISeasonRepository
{
    private readonly EntityContext _entityContext;


    public SeasonRepository(EntityContext entityContext)
    {
        _entityContext = entityContext;
    }

    public Task InsertNewSeasonsAsync(long showId, IEnumerable<Season> seasons, CancellationToken token)
    {
        var currentSeasons = _entityContext.Seasons.Where(season => season.TvShowId == showId).AsNoTracking().Select(season => season.Number).ToFrozenSet();
        return _entityContext.Seasons.BulkInsertAsync(seasons.Where(season => !currentSeasons.Contains(season.Number)), token);
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
        return _entityContext.Seasons.Where(season => season.TvShow.Id == showId).AsNoTracking().ToAsyncEnumerable();
    }

    /// <summary>
    ///     Update the season
    /// </summary>
    public Task SaveChangesAsync(CancellationToken token)
    {
        return _entityContext.SaveChangesAsync(token);
    }
}