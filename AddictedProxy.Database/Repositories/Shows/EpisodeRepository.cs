#region

using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using Microsoft.EntityFrameworkCore;
using Z.BulkOperations;

#endregion

namespace AddictedProxy.Database.Repositories.Shows;

public class EpisodeRepository : IEpisodeRepository
{
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
                        bulkSub.IgnoreOnMergeUpdateExpression = subtitle => new { subtitle.Discovered, subtitle.StoragePath, subtitle.StoredAt, subtitle.DownloadCount, subtitle.UniqueId };
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
    /// Get season episodes
    /// </summary>
    /// <param name="tvShowId"></param>
    /// <param name="season"></param>
    /// <returns></returns>
    public IAsyncEnumerable<Episode> GetSeasonEpisodesAsync(long tvShowId, int season)
    {
        return _entityContext.Episodes.Where(episode => episode.Season == season)
                             .Where(episode => episode.TvShow.Id == tvShowId)
                             .Include(episode => episode.TvShow)
                             .Include(episode => episode.Subtitles)
                             .ToAsyncEnumerable();
    }

    /// <summary>
    ///     Get a specific episode
    /// </summary>
    public Task<Episode?> GetEpisodeUntrackedAsync(long tvShowId, int season, int episodeNumber, CancellationToken token)
    {
        return _entityContext.Episodes
                             .Where(episode => episode.Number == episodeNumber)
                             .Where(episode => episode.Season == season)
                             .Where(episode => episode.TvShow.Id == tvShowId)
                             .Include(episode => episode.TvShow)
                             .Include(episode => episode.Subtitles)
                             .AsNoTracking()
                             .FirstOrDefaultAsync(token);
    }
}