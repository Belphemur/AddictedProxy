using AddictedProxy.Database.Context;
using AddictedProxy.Model.Shows;
using Microsoft.EntityFrameworkCore;
using Z.BulkOperations;

namespace AddictedProxy.Database.Repositories;

public class EpisodeRepository : IEpisodeRepository
{
    private static readonly Action<BulkOperation<Episode>> AvoidUpdateDiscoveredFieldEpisode = Rule.AvoidUpdateDiscoveredField<Episode>();
    private static readonly Action<BulkOperation<Subtitle>> AvoidUpdateDiscoveredFieldSubtitle = Rule.AvoidUpdateDiscoveredField<Subtitle>();
    private readonly EntityContext _entityContext;


    public EpisodeRepository(EntityContext entityContext)
    {
        _entityContext = entityContext;
    }

    /// <summary>
    ///     Upsert the episodes
    /// </summary>
    public async Task UpsertEpisodes(IEnumerable<Episode> episodes, CancellationToken token)
    {
        var transaction = await _entityContext.Database.BeginTransactionAsync(token);

        var enumerable = episodes as Episode[] ?? episodes.ToArray();
        await _entityContext.Episodes.BulkMergeAsync(enumerable, options =>
        {
            options.IncludeGraph = true;
            options.IncludeGraphOperationBuilder = operation =>
            {
                switch (operation)
                {
                    case BulkOperation<Subtitle> bulkSub:
                        bulkSub.IgnoreOnMergeUpdateExpression = subtitle => subtitle.Discovered;
                        bulkSub.ColumnPrimaryKeyExpression = subtitle => new { subtitle.EpisodeId, subtitle.Language, subtitle.Version };
                        break;
                    case BulkOperation<Episode> bulkEp:
                        bulkEp.ColumnPrimaryKeyExpression = episode => new { episode.TvShowId, episode.Season, episode.Number };
                        bulkEp.IgnoreOnMergeUpdateExpression = episode => episode.Discovered;
                        break;
                    case BulkOperation<TvShow> bulkShow:
                        bulkShow.IsReadOnly = true;
                        break;
                }
            };
        }, token);

        await transaction.CommitAsync(token);
    }

    /// <summary>
    ///     Get a specific episode
    /// </summary>
    public Task<Episode?> GetEpisodeAsync(int tvShowId, int season, int episodeNumber, CancellationToken token)
    {
        return _entityContext.Episodes
                             .AsNoTracking()
                             .Include(episode => episode.TvShow)
                             .Include(episode => episode.Subtitles)
                             .Where(episode => episode.Number == episodeNumber)
                             .Where(episode => episode.Season == season)
                             .Where(episode => episode.TvShow.Id == tvShowId)
                             .FirstOrDefaultAsync(token);
    }
}