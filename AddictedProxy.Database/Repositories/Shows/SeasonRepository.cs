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

    public Task InsertNewSeasonsAsync(long showId, IEnumerable<Season> seasons, CancellationToken token)
    {
        var currentSeasons = _entityContext.Seasons.Where(season => season.TvShowId == showId).AsNoTracking().Select(season => season.Number).ToHashSet();
        return _entityContext.Seasons.BulkInsertAsync(seasons.Where(season => !currentSeasons.Contains(season.Number)), token);
    }

    public Task<Season?> GetSeasonForShowAsync(long showId, int seasonNumber, CancellationToken token)
    {
        return _entityContext.Seasons.Where(season => season.TvShow.Id == showId).SingleOrDefaultAsync(season => season.Number == seasonNumber, token);
    }

    /// <summary>
    ///     Update the season
    /// </summary>
    public Task SaveChangesAsync(CancellationToken token)
    {
        return _entityContext.SaveChangesAsync(token);
    }
}