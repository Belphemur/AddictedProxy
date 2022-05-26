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

    public Task UpsertSeasonAsync(IEnumerable<Season> seasons, CancellationToken token)
    {
        return _entityContext.Seasons.BulkMergeAsync(seasons, options =>
        {
            options.ColumnPrimaryKeyExpression = season => new { season.TvShowId, season.Number };
            options.IgnoreOnMergeUpdateExpression = season => new { season.LastRefreshed };
        }, token);
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