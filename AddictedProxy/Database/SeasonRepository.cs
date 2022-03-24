using AddictedProxy.Model.Shows;

namespace AddictedProxy.Database;

public class SeasonRepository : ISeasonRepository
{
    private readonly EntityContext _entityContext;


    public SeasonRepository(EntityContext entityContext)
    {
        _entityContext = entityContext;
    }

    public Task UpsertSeason(IEnumerable<Season> seasons, CancellationToken token)
    {
        return _entityContext.Seasons.BulkMergeAsync(seasons, token);
    }

    public IAsyncEnumerable<Season> GetSeasonsForShow(int showId)
    {
        return _entityContext.Seasons.Where(season => season.TvShowId == showId).ToAsyncEnumerable();
    }

    public Season? GetSeasonForShow(int showId, int seasonNumber)
    {
        return _entityContext.Seasons.Where(season => season.TvShowId == showId).FirstOrDefault(season => season.Number == seasonNumber);
    }
}