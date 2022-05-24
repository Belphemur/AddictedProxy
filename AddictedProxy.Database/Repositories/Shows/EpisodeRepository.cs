#region

using System.Globalization;
using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Transaction;
using Microsoft.EntityFrameworkCore;
using Z.BulkOperations;

#endregion

namespace AddictedProxy.Database.Repositories.Shows;

public class EpisodeRepository : IEpisodeRepository
{
    private readonly EntityContext _entityContext;
    private readonly ITransactionManager _transactionManager;


    public EpisodeRepository(EntityContext entityContext, ITransactionManager transactionManager)
    {
        _entityContext = entityContext;
        _transactionManager = transactionManager;
    }

    /// <summary>
    ///     Upsert the episodes
    /// </summary>
    public async Task UpsertEpisodes(IEnumerable<Episode> episodes, CancellationToken token)
    {
        await using var transaction = await _transactionManager.BeginNestedAsync(token);
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
    /// Get season episodes for language
    /// </summary>
    /// <param name="tvShowId"></param>
    /// <param name="language"></param>
    /// <param name="season"></param>
    /// <returns></returns>
    public IAsyncEnumerable<Episode> GetSeasonEpisodesByLangAsync(long tvShowId, CultureInfo language, int season)
    {
        return _entityContext.Episodes.Where(episode => episode.Season == season)
            .Where(episode => episode.TvShow.Id == tvShowId)
            .Include(episode => episode.TvShow)
            .Include(episode => episode.Subtitles.Where(subtitle => subtitle.Language == language.EnglishName))
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