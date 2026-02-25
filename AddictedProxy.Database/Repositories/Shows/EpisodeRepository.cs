#region

using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Tools.Database.Transaction;
using Microsoft.EntityFrameworkCore;
using Z.BulkOperations;

#endregion

namespace AddictedProxy.Database.Repositories.Shows;

public class EpisodeRepository : IEpisodeRepository
{
    private readonly EntityContext _entityContext;
    private readonly ITransactionManager<EntityContext> _transactionManager;


    public EpisodeRepository(EntityContext entityContext, ITransactionManager<EntityContext> transactionManager)
    {
        _entityContext = entityContext;
        _transactionManager = transactionManager;
    }

    /// <summary>
    ///     Upsert the episodes
    /// </summary>
    public async Task UpsertEpisodes(IEnumerable<Episode> episodes, CancellationToken token)
    {
        await _transactionManager.WrapInTransactionAsync(async () =>
        {
            var enumerable = episodes as Episode[] ?? episodes.ToArray();
            //Nothing to do, no new episodes
            if (enumerable.Length == 0)
            {
                return;
            }

            await _entityContext.Episodes.BulkMergeAsync(enumerable, options =>
            {
                options.IncludeGraph = true;
                options.IncludeGraphOperationBuilder = operation =>
                {
                    switch (operation)
                    {
                        case BulkOperation<Subtitle> bulkSub:
                            bulkSub.IgnoreOnMergeUpdateExpression = subtitle => new { subtitle.Id, subtitle.Discovered, subtitle.CreatedAt, subtitle.StoragePath, subtitle.StoredAt, subtitle.DownloadCount, subtitle.UniqueId, subtitle.Source, subtitle.ExternalId, subtitle.Qualities, subtitle.Release, subtitle.Scene };
                            bulkSub.ColumnPrimaryKeyExpression = subtitle => new { subtitle.DownloadUri };
                            bulkSub.IgnoreOnMergeInsertExpression = subtitle => new { subtitle.Id };
                            break;
                        case BulkOperation<Episode> bulkEp:
                            bulkEp.ColumnPrimaryKeyExpression = episode => new { episode.TvShowId, episode.Season, episode.Number };
                            bulkEp.IgnoreOnMergeUpdateExpression = episode => new { episode.Id, episode.Discovered, episode.CreatedAt };
                            bulkEp.IgnoreOnMergeInsertExpression = episode => episode.Id;
                            break;
                        case BulkOperation<EpisodeExternalId> bulkExtId:
                            bulkExtId.ColumnPrimaryKeyExpression = e => new { e.EpisodeId, e.Source };
                            bulkExtId.IgnoreOnMergeInsertExpression = e => e.Id;
                            bulkExtId.IgnoreOnMergeUpdateExpression = e => new { e.Id, e.CreatedAt };
                            break;
                    }
                };
            }, token);
        }, token);


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
            .OrderBy(episode => episode.Number)
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
    public IAsyncEnumerable<Episode> GetSeasonEpisodesByLangUntrackedAsync(long tvShowId, Culture.Model.Culture language, int season)
    {
        return _entityContext.Episodes.Where(episode => episode.Season == season)
            .Where(episode => episode.TvShow.Id == tvShowId)
            .OrderBy(episode => episode.Number)
            .Include(episode => episode.TvShow)
            .Include(episode => episode.Subtitles.Where(subtitle => subtitle.LanguageIsoCode == language.Name || subtitle.Language == language.EnglishName))
            .AsNoTracking()
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
            .Where(episode => episode.TvShowId == tvShowId)
            .Include(episode => episode.TvShow)
            .Include(episode => episode.Subtitles)
            .AsNoTracking()
            .FirstOrDefaultAsync(token);
    }
}