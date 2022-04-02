using AddictedProxy.Database.Context;
using AddictedProxy.Model.Shows;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Repositories;

public class SeasonRepository : ISeasonRepository
{
    private readonly EntityContext _entityContext;


    public SeasonRepository(EntityContext entityContext)
    {
        _entityContext = entityContext;
    }

    public Task UpsertSeason(IEnumerable<Season> seasons, CancellationToken token)
    {
        return _entityContext.Seasons.BulkMergeAsync(seasons, options => { options.ColumnPrimaryKeyExpression = season => new { season.TvShowId, season.Number }; }, token);
    }

    public IAsyncEnumerable<Season> GetSeasonsForShow(int showId)
    {
        return _entityContext.Seasons.Where(season => season.TvShow.Id == showId).ToAsyncEnumerable();
    }

    public Task<Season?> GetSeasonForShow(int showId, int seasonNumber, CancellationToken token)
    {
        return _entityContext.Seasons.Where(season => season.TvShow.Id == showId).SingleOrDefaultAsync(season => season.Number == seasonNumber, token);
    }

    /// <summary>
    ///     Update the season
    /// </summary>
    public Task UpdateSeasonAsync(Season season, CancellationToken token)
    {
        _entityContext.Seasons.Update(season);
        return _entityContext.SaveChangesAsync(token);
    }
}