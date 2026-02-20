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
                    }
                };
            }, token);
        }, token);


    }

    /// <summary>
    /// Atomically upsert a single episode and its subtitle via SQL.
    /// Returns the database-generated episode ID.
    /// </summary>
    public async Task<long> MergeEpisodeWithSubtitleAsync(Episode episode, Subtitle subtitle, CancellationToken token)
    {
        var now = DateTime.UtcNow;
        long episodeId = 0;

        await _transactionManager.WrapInTransactionAsync(async () =>
        {
            // Step 1: Upsert Episode by (TvShowId, Season, Number).
            // On conflict, update the title; preserve Id, Discovered, CreatedAt.
            episodeId = await _entityContext.Database.SqlQuery<long>(
                $"""
                 INSERT INTO "Episodes" ("TvShowId", "Season", "Number", "Title", "ExternalId", "Discovered", "CreatedAt", "UpdatedAt")
                 VALUES ({episode.TvShowId}, {episode.Season}, {episode.Number}, {episode.Title}, {episode.ExternalId}, {now}, {now}, {now})
                 ON CONFLICT ("TvShowId", "Season", "Number")
                 DO UPDATE SET "Title" = EXCLUDED."Title", "UpdatedAt" = EXCLUDED."UpdatedAt"
                 RETURNING "Id"
                 """).FirstAsync(token);

            // Step 2: Upsert Subtitle by (Source, ExternalId).
            // On conflict, update mutable fields; preserve Id, Discovered, CreatedAt,
            // StoragePath, StoredAt, DownloadCount, UniqueId.
            var downloadUri = subtitle.DownloadUri.ToString();
            await _entityContext.Database.ExecuteSqlAsync(
                $"""
                 INSERT INTO "Subtitles"
                     ("EpisodeId", "Scene", "Version", "Completed", "CompletionPct", "HearingImpaired",
                      "Corrected", "Qualities", "Release", "HD", "DownloadUri", "Language", "LanguageIsoCode",
                      "Source", "ExternalId", "Discovered", "CreatedAt", "UpdatedAt")
                 VALUES
                     ({episodeId}, {subtitle.Scene}, {subtitle.Version}, {subtitle.Completed}, {subtitle.CompletionPct},
                      {subtitle.HearingImpaired}, {subtitle.Corrected}, {(int)subtitle.Qualities}, {subtitle.Release},
                      {false}, {downloadUri}, {subtitle.Language}, {subtitle.LanguageIsoCode},
                      {(int)subtitle.Source}, {subtitle.ExternalId}, {now}, {now}, {now})
                 ON CONFLICT ("Source", "ExternalId")
                 DO UPDATE SET
                     "EpisodeId"        = EXCLUDED."EpisodeId",
                     "DownloadUri"      = EXCLUDED."DownloadUri",
                     "Scene"            = EXCLUDED."Scene",
                     "Version"          = EXCLUDED."Version",
                     "Completed"        = EXCLUDED."Completed",
                     "CompletionPct"    = EXCLUDED."CompletionPct",
                     "HearingImpaired"  = EXCLUDED."HearingImpaired",
                     "Corrected"        = EXCLUDED."Corrected",
                     "Qualities"        = EXCLUDED."Qualities",
                     "Release"          = EXCLUDED."Release",
                     "Language"         = EXCLUDED."Language",
                     "LanguageIsoCode"  = EXCLUDED."LanguageIsoCode",
                     "UpdatedAt"        = EXCLUDED."UpdatedAt"
                 """,
                token);
        }, token);

        return episodeId;
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
    public IAsyncEnumerable<Episode> GetSeasonEpisodesByLangUntrackedAsync(long tvShowId, Culture.Model.Culture language, int season)
    {
        return _entityContext.Episodes.Where(episode => episode.Season == season)
            .Where(episode => episode.TvShow.Id == tvShowId)
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