using AddictedProxy.Culture.Service;
using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.State;
using AddictedProxy.Services.Job.Filter;
using AddictedProxy.Services.Provider.Merging;
using AddictedProxy.Services.Provider.Merging.Model;
using AddictedProxy.Services.Provider.SuperSubtitles;
using AddictedProxy.Tools.Database.Transaction;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using SuperSubtitleClient.Generated;
using SuperSubtitleClient.Service;

using SubtitleEntity = AddictedProxy.Database.Model.Shows.Subtitle;

namespace AddictedProxy.Services.Provider.SuperSubtitles.Jobs;

/// <summary>
/// Recurring Hangfire job that incrementally imports new subtitles from SuperSubtitles every 15 minutes.
/// Uses the max subtitle ID stored by <see cref="ImportSuperSubtitlesJob"/> as a cursor,
/// and only fetches subtitles added since that ID.
/// </summary>
/// <remarks>
/// Skips immediately if the bulk import cursor is not yet set (bulk import not completed).
/// Uses <c>CheckForUpdates</c> as an early-exit gate before streaming.
/// </remarks>
[UniqueJob]
public class RefreshSuperSubtitlesJob
{
    private readonly ISuperSubtitlesClient _superSubtitlesClient;
    private readonly IProviderDataIngestionService _ingestionService;
    private readonly ITransactionManager<EntityContext> _transactionManager;
    private readonly ISuperSubtitlesStateRepository _stateRepository;
    private readonly ICultureParser _cultureParser;
    private readonly ILogger<RefreshSuperSubtitlesJob> _logger;

    public RefreshSuperSubtitlesJob(
        ISuperSubtitlesClient superSubtitlesClient,
        IProviderDataIngestionService ingestionService,
        ITransactionManager<EntityContext> transactionManager,
        ISuperSubtitlesStateRepository stateRepository,
        ICultureParser cultureParser,
        ILogger<RefreshSuperSubtitlesJob> logger)
    {
        _superSubtitlesClient = superSubtitlesClient;
        _ingestionService = ingestionService;
        _transactionManager = transactionManager;
        _stateRepository = stateRepository;
        _cultureParser = cultureParser;
        _logger = logger;
    }

    public async Task ExecuteAsync(PerformContext context, CancellationToken token)
    {
        // Guard: bulk import must have run first to establish the cursor
        var maxId = await _stateRepository.GetMaxSubtitleIdAsync(token);
        if (maxId == 0)
        {
            context.WriteLine("Bulk import cursor not found — bulk import has not completed yet, skipping.");
            _logger.LogInformation("SuperSubtitles incremental refresh skipped: bulk import not yet completed");
            return;
        }

        // Early-exit: check for updates before streaming
        var updateCheck = await _superSubtitlesClient.CheckForUpdatesAsync(maxId, token);
        if (updateCheck.SeriesCount == 0)
        {
            context.WriteLine($"No new series subtitles since ID {maxId}, nothing to do.");
            _logger.LogInformation("SuperSubtitles incremental refresh: no new series subtitles since ID {MaxId}", maxId);
            return;
        }

        context.WriteLine($"New series subtitles available ({updateCheck.SeriesCount} series). Fetching since ID {maxId}...");
        _logger.LogInformation(
            "SuperSubtitles incremental refresh: {SeriesCount} new series subtitles since ID {MaxId}",
            updateCheck.SeriesCount, maxId);

        long newMaxId = maxId;
        var subtitleCount = 0;
        var seasonPackCount = 0;

        await _transactionManager.WrapInTransactionAsync(async () =>
        {
            await foreach (var collection in _superSubtitlesClient.GetRecentSubtitlesAsync(maxId, token))
            {
                var stats = await ProcessShowCollectionAsync(collection, newMaxId, token);
                newMaxId = stats.MaxSubtitleId;
                subtitleCount += stats.SubtitleCount;
                seasonPackCount += stats.SeasonPackCount;
            }
        }, token);

        if (newMaxId > maxId)
        {
            await _stateRepository.SetMaxSubtitleIdAsync(newMaxId, token);
        }

        context.WriteLine($"Refresh complete: {subtitleCount} subtitles, {seasonPackCount} season packs, new max ID: {newMaxId}");
        _logger.LogInformation(
            "SuperSubtitles incremental refresh complete: {SubtitleCount} subtitles, {SeasonPackCount} season packs, new max ID: {NewMaxId}",
            subtitleCount, seasonPackCount, newMaxId);
    }

    private record ProcessingStats(long MaxSubtitleId, int SubtitleCount, int SeasonPackCount);

    private async Task<ProcessingStats> ProcessShowCollectionAsync(ShowSubtitlesCollection collection, long currentMaxId, CancellationToken token)
    {
        // 1. Merge show
        var thirdPartyIds = collection.ShowInfo.ThirdPartyIds != null
            ? new ThirdPartyShowIds(
                TvdbId: collection.ShowInfo.ThirdPartyIds.TvdbId > 0 ? (int)collection.ShowInfo.ThirdPartyIds.TvdbId : null,
                ImdbId: !string.IsNullOrEmpty(collection.ShowInfo.ThirdPartyIds.ImdbId) ? collection.ShowInfo.ThirdPartyIds.ImdbId : null,
                TmdbId: null)
            : null;

        var currentShow = await _ingestionService.MergeShowAsync(
            DataSource.SuperSubtitles,
            collection.ShowInfo.Show.Id.ToString(),
            collection.ShowInfo.Show.Name,
            thirdPartyIds,
            token);

        // 2. Build episodes and season packs from the collection's subtitles
        var now = DateTime.UtcNow;
        var seasonPacks = new List<SeasonPackSubtitle>();
        var subtitlesByEpisode = new Dictionary<(int Season, int Episode), (string Title, List<SubtitleEntity> Subtitles)>();
        long maxSubtitleId = currentMaxId;
        var languageCache = new Dictionary<string, string?>();

        foreach (var subtitle in collection.Subtitles)
        {
            maxSubtitleId = Math.Max(maxSubtitleId, subtitle.Id);
            var languageIsoCode = await GetOrCacheLanguageIsoCodeAsync(subtitle.Language, languageCache, token);

            if (subtitle.IsSeasonPack)
            {
                seasonPacks.Add(subtitle.ToSeasonPackSubtitle(currentShow.Id, languageIsoCode));
            }
            else
            {
                var key = (subtitle.Season, subtitle.Episode);
                if (!subtitlesByEpisode.TryGetValue(key, out var group))
                {
                    group = (subtitle.Name ?? string.Empty, new List<SubtitleEntity>());
                    subtitlesByEpisode[key] = group;
                }
                group.Subtitles.Add(subtitle.ToSubtitleEntity(languageIsoCode));
            }
        }

        // 3. Build Episode entities with their Subtitles and ExternalIds
        var showExternalId = collection.ShowInfo.Show.Id;
        var episodes = subtitlesByEpisode.Select(kvp => new Episode
        {
            TvShowId = currentShow.Id,
            Season = kvp.Key.Season,
            Number = kvp.Key.Episode,
            Title = kvp.Value.Title,
            Discovered = now,
            Subtitles = kvp.Value.Subtitles,
            ExternalIds = [new EpisodeExternalId
            {
                Source = DataSource.SuperSubtitles,
                ExternalId = $"{showExternalId}-S{kvp.Key.Season}E{kvp.Key.Episode}"
            }]
        }).ToList();

        // 4. Bulk upsert episodes (with subtitles + external IDs) and season packs
        await _ingestionService.MergeEpisodesWithSubtitlesAsync(currentShow, episodes, token);

        if (seasonPacks.Count > 0)
        {
            await _ingestionService.IngestSeasonPacksAsync(seasonPacks, token);
        }

        var subtitleCount = subtitlesByEpisode.Values.Sum(g => g.Subtitles.Count);
        return new ProcessingStats(maxSubtitleId, subtitleCount, seasonPacks.Count);
    }

    private async Task<string?> GetOrCacheLanguageIsoCodeAsync(string language, Dictionary<string, string?> cache, CancellationToken token)
    {
        if (cache.TryGetValue(language, out var cached))
            return cached;

        var culture = await _cultureParser.FromStringAsync(language, token);
        var isoCode = culture?.TwoLetterISOLanguageName;
        cache[language] = isoCode;
        return isoCode;
    }
}
