using AddictedProxy.Culture.Service;
using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.State;
using AddictedProxy.Services.Job.Filter;
using AddictedProxy.Services.Job.Service;
using AddictedProxy.Services.Provider.Merging;
using AddictedProxy.Services.Provider.Merging.Model;
using AddictedProxy.Services.Provider.SuperSubtitles.Config;
using AddictedProxy.Tools.Database.Transaction;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SuperSubtitleClient.Generated;
using SuperSubtitleClient.Service;

using ProtoShowInfo = SuperSubtitleClient.Generated.ShowInfo;
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
        TvShow? currentShow = null;
        var maxSubtitleId = currentMaxId;
        var subtitleCount = 0;
        var seasonPackCount = 0;

        await _transactionManager.WrapInTransactionAsync(async () =>
        {
            await foreach (var item in _superSubtitlesClient.GetShowSubtitlesAsync(batch, token))
            {
                switch (item.ItemCase)
                {
                    case ShowSubtitleItem.ItemOneofCase.ShowInfo:
                        currentShow = await HandleShowInfoAsync(item.ShowInfo, token);
                        break;

                    case ShowSubtitleItem.ItemOneofCase.Subtitle:
                        if (currentShow == null)
                        {
                            _logger.LogWarning(
                                "Received subtitle {SubtitleId} without preceding ShowInfo, skipping",
                                item.Subtitle.Id);
                            break;
                        }

                        var isSeasonPack = await HandleSubtitleAsync(item.Subtitle, currentShow, token);
                        maxSubtitleId = Math.Max(maxSubtitleId, item.Subtitle.Id);
                        if (isSeasonPack)
                            seasonPackCount++;
                        else
                            subtitleCount++;
                        break;

                    case ShowSubtitleItem.ItemOneofCase.None:
                        _logger.LogWarning("Received ShowSubtitleItem with no data");
                        break;

                    default:
                        _logger.LogWarning("Unknown ShowSubtitleItem case: {Case}", item.ItemCase);
                        break;
                }
            }
        }, token);

        return new BatchStats(maxSubtitleId, subtitleCount, seasonPackCount);
    }

    private async Task<TvShow> HandleShowInfoAsync(ProtoShowInfo showInfo, CancellationToken token)
    {
        var thirdPartyIds = showInfo.ThirdPartyIds != null
            ? new ThirdPartyShowIds(
                TvdbId: showInfo.ThirdPartyIds.TvdbId > 0 ? (int)showInfo.ThirdPartyIds.TvdbId : null,
                ImdbId: !string.IsNullOrEmpty(showInfo.ThirdPartyIds.ImdbId) ? showInfo.ThirdPartyIds.ImdbId : null,
                TmdbId: null)
            : null;

        return await _ingestionService.MergeShowAsync(
            DataSource.SuperSubtitles,
            showInfo.Show.Id.ToString(),
            showInfo.Show.Name,
            thirdPartyIds,
            token);
    }

    /// <summary>
    /// Process a single subtitle from the stream.
    /// Returns true if the subtitle was a season pack, false otherwise.
    /// </summary>
    private async Task<bool> HandleSubtitleAsync(ProtoSubtitle subtitle, TvShow currentShow, CancellationToken token)
    {
        if (subtitle.IsSeasonPack)
        {
            await HandleSeasonPackAsync(currentShow, subtitle, token);
            return true;
        }

        await HandleEpisodeSubtitleAsync(currentShow, subtitle, token);
        return false;
    }

    private async Task HandleSeasonPackAsync(TvShow tvShow, ProtoSubtitle subtitle, CancellationToken token)
    {
        var culture = await _cultureParser.FromStringAsync(subtitle.Language, token);

        var seasonPack = new SeasonPackSubtitle
        {
            TvShowId = tvShow.Id,
            Season = subtitle.Season,
            Source = DataSource.SuperSubtitles,
            ExternalId = subtitle.Id,
            Filename = subtitle.Filename,
            Language = subtitle.Language,
            LanguageIsoCode = culture?.TwoLetterISOLanguageName,
            Release = string.IsNullOrEmpty(subtitle.Release) ? null : subtitle.Release,
            Uploader = string.IsNullOrEmpty(subtitle.Uploader) ? null : subtitle.Uploader,
            UploadedAt = subtitle.UploadedAt?.ToDateTime(),
            Qualities = ToVideoQuality(subtitle.Qualities),
            ReleaseGroups = subtitle.ReleaseGroups.Count > 0
                ? string.Join(",", subtitle.ReleaseGroups)
                : null,
            Discovered = DateTime.UtcNow
        };

        await _ingestionService.IngestSeasonPackAsync(seasonPack, token);
    }

    private async Task HandleEpisodeSubtitleAsync(TvShow tvShow, ProtoSubtitle subtitle, CancellationToken token)
    {
        var culture = await _cultureParser.FromStringAsync(subtitle.Language, token);

        var subtitleEntity = new SubtitleEntity
        {
            Scene = string.Join(", ", subtitle.ReleaseGroups),
            Version = 0,
            Completed = true,
            CompletionPct = 100.0,
            HearingImpaired = false,
            Corrected = false,
            Qualities = ToVideoQuality(subtitle.Qualities),
            Release = string.IsNullOrEmpty(subtitle.Release) ? null : subtitle.Release,
            DownloadUri = new Uri(subtitle.DownloadUrl),
            Language = subtitle.Language,
            LanguageIsoCode = culture?.TwoLetterISOLanguageName,
            Discovered = DateTime.UtcNow,
            Source = DataSource.SuperSubtitles,
            ExternalId = subtitle.Id.ToString()
        };

        await _ingestionService.MergeEpisodeSubtitleAsync(
            tvShow,
            DataSource.SuperSubtitles,
            subtitle.Season,
            subtitle.Episode,
            subtitle.Name,
            subtitle.Id.ToString(),
            subtitleEntity,
            token);
    }

    private static VideoQuality ToVideoQuality(IEnumerable<Quality> protoQualities)
    {
        var result = VideoQuality.None;
        foreach (var q in protoQualities)
        {
            result |= q switch
            {
                Quality._360P  => VideoQuality.Q360P,
                Quality._480P  => VideoQuality.Q480P,
                Quality._720P  => VideoQuality.Q720P,
                Quality._1080P => VideoQuality.Q1080P,
                Quality._2160P => VideoQuality.Q2160P,
                _              => VideoQuality.None,
            };
        }
        return result;
    }
}
