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
    /// Atomically upsert a single episode, its subtitle, and optionally its external ID via SQL.
    /// Returns the database-generated episode ID.
    /// </summary>
    public async Task<long> MergeEpisodeWithSubtitleAsync(Episode episode, Subtitle subtitle, CancellationToken token)
    {
        var now = DateTime.UtcNow;
        var downloadUri = subtitle.DownloadUri.ToString();
        var source = (int)subtitle.Source;
        var episodeExternalId = episode.ExternalIds.FirstOrDefault()?.ExternalId;

        // Top-level CTE: upsert episode, then subtitle, then episode external ID.
        // ExecuteSqlAsync sends the SQL directly without wrapping in a subquery,
        // which is required because PostgreSQL forbids data-modifying CTEs inside subqueries.
        //
        // The episode external ID CTEs use a WHERE guard ({episodeExternalId} IS NOT NULL)
        // so they become no-ops when no external ID is provided.
        await _entityContext.Database.ExecuteSqlAsync(
            $"""
             WITH upsert_episode AS (
                 INSERT INTO "Episodes" ("TvShowId", "Season", "Number", "Title", "Discovered", "CreatedAt", "UpdatedAt")
                 VALUES ({episode.TvShowId}, {episode.Season}, {episode.Number}, {episode.Title}, {now}, {now}, {now})
                 ON CONFLICT ("TvShowId", "Season", "Number")
                 DO UPDATE SET "Title" = EXCLUDED."Title", "UpdatedAt" = EXCLUDED."UpdatedAt"
                 RETURNING "Id"
             ),
             upsert_subtitle AS (
                 INSERT INTO "Subtitles"
                     ("EpisodeId", "Scene", "Version", "Completed", "CompletionPct", "HearingImpaired",
                      "Corrected", "Qualities", "Release", "HD", "DownloadUri", "Language", "LanguageIsoCode",
                      "Source", "ExternalId", "DownloadCount", "Discovered", "CreatedAt", "UpdatedAt")
                 SELECT
                     "Id", {subtitle.Scene}, {subtitle.Version}, {subtitle.Completed}, {subtitle.CompletionPct},
                     {subtitle.HearingImpaired}, {subtitle.Corrected}, {(int)subtitle.Qualities}, {subtitle.Release},
                     {false}, {downloadUri}, {subtitle.Language}, {subtitle.LanguageIsoCode},
                     {source}, {subtitle.ExternalId}, {0L}, {now}, {now}, {now}
                 FROM upsert_episode
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
             ),
             update_episode_ext_id AS (
                 UPDATE "EpisodeExternalIds"
                 SET "ExternalId" = {episodeExternalId}, "UpdatedAt" = {now}
                 FROM upsert_episode
                 WHERE "EpisodeExternalIds"."EpisodeId" = upsert_episode."Id"
                   AND "EpisodeExternalIds"."Source" = {source}
                   AND {episodeExternalId} IS NOT NULL
                   AND NOT EXISTS (
                       SELECT 1 FROM "EpisodeExternalIds" e2
                       WHERE e2."Source" = {source}
                         AND e2."ExternalId" = {episodeExternalId}
                         AND e2."EpisodeId" != upsert_episode."Id"
                   )
                 RETURNING "EpisodeExternalIds"."Id"
             )
             INSERT INTO "EpisodeExternalIds" ("EpisodeId", "Source", "ExternalId", "CreatedAt", "UpdatedAt")
             SELECT "Id", {source}, {episodeExternalId}, {now}, {now}
             FROM upsert_episode
             WHERE {episodeExternalId} IS NOT NULL
               AND NOT EXISTS (SELECT 1 FROM update_episode_ext_id)
             ON CONFLICT ("Source", "ExternalId") DO NOTHING
             """, token);

        // Retrieve the episode ID (runs in the same transaction as the upsert above)
        return await _entityContext.Episodes
            .Where(e => e.TvShowId == episode.TvShowId && e.Season == episode.Season && e.Number == episode.Number)
            .Select(e => e.Id)
            .FirstAsync(token);
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