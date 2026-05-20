#region

using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Tools.Database.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Z.BulkOperations;

#endregion

namespace AddictedProxy.Database.Repositories.Shows;

public class EpisodeRepository : IEpisodeRepository
{
    private readonly EntityContext _entityContext;
    private readonly ITransactionManager<EntityContext> _transactionManager;
    private readonly ILogger<EpisodeRepository> _logger;


    public EpisodeRepository(EntityContext entityContext, ITransactionManager<EntityContext> transactionManager, ILogger<EpisodeRepository> logger)
    {
        _entityContext = entityContext;
        _transactionManager = transactionManager;
        _logger = logger;
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

            var originalExternalIds = enumerable.ToDictionary(episode => episode,
                episode => episode.ExternalIds?.ToArray() ?? []);

            foreach (var episode in enumerable)
            {
                episode.ExternalIds = [];
            }

            try
            {
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

                var pendingExternalIds = BuildPendingEpisodeExternalIdsForUpsert(enumerable, originalExternalIds, _logger);
                if (pendingExternalIds.Count == 0)
                {
                    return;
                }

                var persistedEpisodes = await LoadPersistedEpisodesAsync(enumerable, token);

                foreach (var externalId in pendingExternalIds)
                {
                    if (!persistedEpisodes.TryGetValue(externalId.EpisodeKey, out var persistedEpisode))
                    {
                        _logger.LogWarning(
                            "Skipping episode external ID because the persisted episode could not be resolved after bulk merge. Show={ShowName}, TvShowId={TvShowId}, Season={Season}, Episode={EpisodeNumber}, Source={Source}, ExternalId={ExternalId}",
                            externalId.ShowName,
                            externalId.EpisodeKey.TvShowId,
                            externalId.EpisodeKey.Season,
                            externalId.EpisodeKey.Number,
                            externalId.Source,
                            externalId.ExternalId);
                        continue;
                    }

                    await UpsertEpisodeExternalIdAsync(externalId, persistedEpisode, token);
                }
            }
            finally
            {
                foreach (var (episode, externalIds) in originalExternalIds)
                {
                    episode.ExternalIds = externalIds;
                }
            }
        }, token);


    }

    internal static IReadOnlyList<PendingEpisodeExternalId> BuildPendingEpisodeExternalIdsForUpsert(
        IEnumerable<Episode> episodes,
        IReadOnlyDictionary<Episode, EpisodeExternalId[]> originalExternalIds,
        ILogger<EpisodeRepository> logger)
    {
        var pendingExternalIds = new List<PendingEpisodeExternalId>();
        var seenExternalIds = new Dictionary<(DataSource Source, string ExternalId), PendingEpisodeExternalId>();

        foreach (var episode in episodes)
        {
            if (!originalExternalIds.TryGetValue(episode, out var externalIds))
            {
                continue;
            }

            foreach (var externalId in externalIds)
            {
                if (string.IsNullOrWhiteSpace(externalId.ExternalId))
                {
                    logger.LogWarning(
                        "Skipping episode external ID with empty value during batch upsert. Show={ShowName}, TvShowId={TvShowId}, Season={Season}, Episode={EpisodeNumber}, Source={Source}",
                        episode.TvShow?.Name ?? episode.Title,
                        episode.TvShowId,
                        episode.Season,
                        episode.Number,
                        externalId.Source);
                    continue;
                }

                var pending = new PendingEpisodeExternalId(
                    new EpisodeNaturalKey(episode.TvShowId, episode.Season, episode.Number),
                    episode.TvShow?.Name ?? episode.Title,
                    externalId.Source,
                    externalId.ExternalId);

                var externalKey = (pending.Source, pending.ExternalId);
                if (seenExternalIds.TryGetValue(externalKey, out var existing))
                {
                    if (existing.EpisodeKey != pending.EpisodeKey)
                    {
                        logger.LogWarning(
                            "Skipping duplicate episode external ID in batch upsert. Source={Source}, ExternalId={ExternalId}, FirstShow={FirstShow}, FirstSeason={FirstSeason}, FirstEpisode={FirstEpisode}, DuplicateShow={DuplicateShow}, DuplicateSeason={DuplicateSeason}, DuplicateEpisode={DuplicateEpisode}",
                            pending.Source,
                            pending.ExternalId,
                            existing.ShowName,
                            existing.EpisodeKey.Season,
                            existing.EpisodeKey.Number,
                            pending.ShowName,
                            pending.EpisodeKey.Season,
                            pending.EpisodeKey.Number);
                    }

                    continue;
                }

                seenExternalIds[externalKey] = pending;
                pendingExternalIds.Add(pending);
            }
        }

        return pendingExternalIds;
    }

    private async Task<Dictionary<EpisodeNaturalKey, PersistedEpisodeInfo>> LoadPersistedEpisodesAsync(IEnumerable<Episode> episodes, CancellationToken token)
    {
        var episodeArray = episodes as Episode[] ?? episodes.ToArray();
        var episodeKeys = episodeArray
            .Select(episode => new EpisodeNaturalKey(episode.TvShowId, episode.Season, episode.Number))
            .Distinct()
            .ToHashSet();
        var tvShowIds = episodeKeys.Select(key => key.TvShowId).Distinct().ToArray();
        var seasons = episodeKeys.Select(key => key.Season).Distinct().ToArray();

        var persistedEpisodes = await _entityContext.Episodes
            .AsNoTracking()
            .Where(episode => tvShowIds.Contains(episode.TvShowId))
            .Where(episode => seasons.Contains(episode.Season))
            .Select(episode => new PersistedEpisodeInfo(
                episode.Id,
                new EpisodeNaturalKey(episode.TvShowId, episode.Season, episode.Number),
                episode.TvShow.Name,
                episode.Season,
                episode.Number))
            .ToListAsync(token);

        return persistedEpisodes
            .Where(episode => episodeKeys.Contains(episode.EpisodeKey))
            .ToDictionary(episode => episode.EpisodeKey);
    }

    private async Task UpsertEpisodeExternalIdAsync(
        PendingEpisodeExternalId externalId,
        PersistedEpisodeInfo persistedEpisode,
        CancellationToken token)
    {
        var conflictingExternalId = await _entityContext.EpisodeExternalIds
            .AsNoTracking()
            .Where(mapping => mapping.Source == externalId.Source && mapping.ExternalId == externalId.ExternalId)
            .Select(mapping => new PersistedEpisodeInfo(
                mapping.EpisodeId,
                new EpisodeNaturalKey(mapping.Episode.TvShowId, mapping.Episode.Season, mapping.Episode.Number),
                mapping.Episode.TvShow.Name,
                mapping.Episode.Season,
                mapping.Episode.Number))
            .FirstOrDefaultAsync(token);

        if (conflictingExternalId is not null && conflictingExternalId.Id != persistedEpisode.Id)
        {
            _logger.LogWarning(
                "Episode external ID already belongs to another episode. Source={Source}, ExternalId={ExternalId}, ExistingShow={ExistingShow}, ExistingSeason={ExistingSeason}, ExistingEpisode={ExistingEpisode}, RequestedShow={RequestedShow}, RequestedSeason={RequestedSeason}, RequestedEpisode={RequestedEpisode}",
                externalId.Source,
                externalId.ExternalId,
                conflictingExternalId.ShowName,
                conflictingExternalId.Season,
                conflictingExternalId.Number,
                persistedEpisode.ShowName,
                persistedEpisode.Season,
                persistedEpisode.Number);
        }

        var now = DateTime.UtcNow;
        await _entityContext.Database.ExecuteSqlAsync(
            $"""
             WITH update_by_episode AS (
                 UPDATE "EpisodeExternalIds"
                 SET "ExternalId" = {externalId.ExternalId}, "UpdatedAt" = {now}
                 WHERE "EpisodeId" = {persistedEpisode.Id}
                   AND "Source" = {(int)externalId.Source}
                   AND NOT EXISTS (
                       SELECT 1 FROM "EpisodeExternalIds" e2
                       WHERE e2."Source" = {(int)externalId.Source}
                         AND e2."ExternalId" = {externalId.ExternalId}
                         AND e2."EpisodeId" != {persistedEpisode.Id}
                   )
                 RETURNING "Id"
             )
             INSERT INTO "EpisodeExternalIds" ("EpisodeId", "Source", "ExternalId", "CreatedAt", "UpdatedAt")
             SELECT {persistedEpisode.Id}, {(int)externalId.Source}, {externalId.ExternalId}, {now}, {now}
             WHERE NOT EXISTS (SELECT 1 FROM update_by_episode)
             ON CONFLICT ("Source", "ExternalId") DO NOTHING
             """,
            token);
    }

    internal sealed record EpisodeNaturalKey(long TvShowId, int Season, int Number);

    internal sealed record PendingEpisodeExternalId(
        EpisodeNaturalKey EpisodeKey,
        string ShowName,
        DataSource Source,
        string ExternalId);

    private sealed record PersistedEpisodeInfo(
        long Id,
        EpisodeNaturalKey EpisodeKey,
        string ShowName,
        int Season,
        int Number);

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
