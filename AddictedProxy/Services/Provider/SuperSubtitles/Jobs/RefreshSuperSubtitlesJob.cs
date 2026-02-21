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

using ProtoShowInfo = SuperSubtitleClient.Generated.ShowInfo;
using ProtoSubtitle = SuperSubtitleClient.Generated.Subtitle;
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
                var currentShow = await HandleShowInfoAsync(collection.ShowInfo, token);

                foreach (var subtitle in collection.Subtitles)
                {
                    var isSeasonPack = await HandleSubtitleAsync(subtitle, currentShow, token);
                    newMaxId = Math.Max(newMaxId, subtitle.Id);
                    if (isSeasonPack)
                        seasonPackCount++;
                    else
                        subtitleCount++;
                }
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
    /// Process a single subtitle from the collection.
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
            Qualities = subtitle.Qualities.ToVideoQuality(),
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
            Qualities = subtitle.Qualities.ToVideoQuality(),
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
}
