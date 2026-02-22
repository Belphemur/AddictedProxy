using AddictedProxy.Culture.Service;
using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.State;
using AddictedProxy.Services.Job.Filter;
using AddictedProxy.Services.Job.Service;
using AddictedProxy.Services.Provider.Merging;
using AddictedProxy.Services.Provider.Merging.Model;
using AddictedProxy.Services.Provider.SuperSubtitles;
using AddictedProxy.Services.Provider.SuperSubtitles.Config;
using AddictedProxy.Tools.Database.Transaction;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SuperSubtitleClient.Generated;
using SuperSubtitleClient.Service;

using ProtoSubtitle = SuperSubtitleClient.Generated.Subtitle;
using SubtitleEntity = AddictedProxy.Database.Model.Shows.Subtitle;

namespace AddictedProxy.Services.Provider.SuperSubtitles.Jobs;

/// <summary>
/// One-time Hangfire job that bulk-imports all shows and subtitles from SuperSubtitles.
/// Fetches all shows via gRPC streaming, processes them in batches,
/// and stores the max subtitle ID as a cursor for incremental updates.
/// </summary>
/// <remarks>
/// Idempotent: checks if a max subtitle ID cursor already exists and skips if so.
/// Enqueued once at startup from <see cref="SchedulerHostedService"/>.
/// </remarks>
[UniqueJob]
public class ImportSuperSubtitlesJob
{
    private readonly ISuperSubtitlesClient _superSubtitlesClient;
    private readonly IProviderDataIngestionService _ingestionService;
    private readonly ITransactionManager<EntityContext> _transactionManager;
    private readonly ISuperSubtitlesStateRepository _stateRepository;
    private readonly ICultureParser _cultureParser;
    private readonly ILogger<ImportSuperSubtitlesJob> _logger;
    private readonly SuperSubtitlesImportConfig _config;

    public ImportSuperSubtitlesJob(
        ISuperSubtitlesClient superSubtitlesClient,
        IProviderDataIngestionService ingestionService,
        ITransactionManager<EntityContext> transactionManager,
        ISuperSubtitlesStateRepository stateRepository,
        ICultureParser cultureParser,
        IOptions<SuperSubtitlesImportConfig> config,
        ILogger<ImportSuperSubtitlesJob> logger)
    {
        _superSubtitlesClient = superSubtitlesClient;
        _ingestionService = ingestionService;
        _transactionManager = transactionManager;
        _stateRepository = stateRepository;
        _cultureParser = cultureParser;
        _logger = logger;
        _config = config.Value;
    }

    public async Task ExecuteAsync(PerformContext context, CancellationToken token)
    {
        // Idempotency: skip if already imported
        var existingMaxId = await _stateRepository.GetMaxSubtitleIdAsync(token);
        if (existingMaxId > 0)
        {
            context.WriteLine($"Bulk import already completed (max subtitle ID: {existingMaxId}), skipping.");
            _logger.LogInformation(
                "SuperSubtitles bulk import already completed (max subtitle ID: {MaxId}), skipping",
                existingMaxId);
            return;
        }

        context.WriteLine("Starting SuperSubtitles bulk import...");
        _logger.LogInformation("Starting SuperSubtitles bulk import...");

        // 1. Fetch all shows via gRPC streaming
        context.WriteLine("Fetching show list from SuperSubtitles...");
        var allShows = await CollectShowsAsync(token);
        context.WriteLine($"Collected {allShows.Count} shows.");
        _logger.LogInformation("Collected {ShowCount} shows", allShows.Count);

        // 2. Split into batches
        var batches = allShows.Chunk(_config.BatchSize).ToArray();
        context.WriteLine($"Split into {batches.Length} batches of {_config.BatchSize} shows each.");
        var progressBar = context.WriteProgressBar();

        long maxSubtitleId = 0;
        var totalSubtitles = 0;
        var totalSeasonPacks = 0;

        for (var i = 0; i < batches.Length; i++)
        {
            var batch = batches[i];
            context.WriteLine($"Processing batch {i + 1}/{batches.Length}...");

            // 3. Process batch
            var batchStats = await ProcessShowBatchAsync(batch, maxSubtitleId, token);
            maxSubtitleId = batchStats.MaxSubtitleId;
            totalSubtitles += batchStats.SubtitleCount;
            totalSeasonPacks += batchStats.SeasonPackCount;

            progressBar.SetValue((i + 1) * 100.0 / batches.Length);
            context.WriteLine(
                $"Batch {i + 1} complete: {batchStats.SubtitleCount} subtitles, {batchStats.SeasonPackCount} season packs (max ID: {maxSubtitleId})");

            // 4. Rate-limit between batches
            if (i < batches.Length - 1)
            {
                var delaySeconds = Random.Shared.Next(_config.MinDelaySeconds, _config.MaxDelaySeconds + 1);
                context.WriteLine($"Waiting {delaySeconds}s before next batch...");
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds), token);
            }
        }

        // 5. Store max ID for incremental updates
        await _stateRepository.SetMaxSubtitleIdAsync(maxSubtitleId, token);

        context.WriteLine($"Import complete! Total: {totalSubtitles} subtitles, {totalSeasonPacks} season packs, max subtitle ID: {maxSubtitleId}");
        _logger.LogInformation(
            "Import complete! Total: {SubtitleCount} subtitles, {SeasonPackCount} season packs, max subtitle ID: {MaxSubtitleId}",
            totalSubtitles, totalSeasonPacks, maxSubtitleId);
    }

    private async Task<List<Show>> CollectShowsAsync(CancellationToken token)
    {
        var shows = new List<Show>();
        await foreach (var show in _superSubtitlesClient.GetShowListAsync(token))
        {
            shows.Add(show);
        }

        return shows;
    }

    private record BatchStats(long MaxSubtitleId, int SubtitleCount, int SeasonPackCount);

    private async Task<BatchStats> ProcessShowBatchAsync(Show[] batch, long currentMaxId, CancellationToken token)
    {
        var maxSubtitleId = currentMaxId;
        var subtitleCount = 0;
        var seasonPackCount = 0;

        await _transactionManager.WrapInTransactionAsync(async () =>
        {
            await foreach (var collection in _superSubtitlesClient.GetShowSubtitlesAsync(batch, token))
            {
                var stats = await ProcessShowCollectionAsync(collection, maxSubtitleId, token);
                maxSubtitleId = stats.MaxSubtitleId;
                subtitleCount += stats.SubtitleCount;
                seasonPackCount += stats.SeasonPackCount;
            }
        }, token);

        return new BatchStats(maxSubtitleId, subtitleCount, seasonPackCount);
    }

    private async Task<BatchStats> ProcessShowCollectionAsync(ShowSubtitlesCollection collection, long currentMaxId, CancellationToken token)
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
        return new BatchStats(maxSubtitleId, subtitleCount, seasonPacks.Count);
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
