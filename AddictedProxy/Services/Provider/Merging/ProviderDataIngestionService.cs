using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Provider.Merging.Model;
using Microsoft.Extensions.Logging;
using TvMovieDatabaseClient.Model.Mapping;
using TvMovieDatabaseClient.Service;

using SubtitleEntity = AddictedProxy.Database.Model.Shows.Subtitle;

namespace AddictedProxy.Services.Provider.Merging;

/// <summary>
/// Service that merges provider data into the unified database model.
/// Handles the core data merging strategy described in the multi-provider architecture plan (Phase 2).
/// </summary>
public class ProviderDataIngestionService : IProviderDataIngestionService
{
    private readonly IShowExternalIdRepository _showExternalIdRepo;
    private readonly IEpisodeExternalIdRepository _episodeExternalIdRepo;
    private readonly ITvShowRepository _tvShowRepo;
    private readonly IEpisodeRepository _episodeRepo;
    private readonly ISeasonRepository _seasonRepo;
    private readonly ISeasonPackSubtitleRepository _seasonPackRepo;
    private readonly ITMDBClient _tmdbClient;
    private readonly ILogger<ProviderDataIngestionService> _logger;

    public ProviderDataIngestionService(
        IShowExternalIdRepository showExternalIdRepo,
        IEpisodeExternalIdRepository episodeExternalIdRepo,
        ITvShowRepository tvShowRepo,
        IEpisodeRepository episodeRepo,
        ISeasonRepository seasonRepo,
        ISeasonPackSubtitleRepository seasonPackRepo,
        ITMDBClient tmdbClient,
        ILogger<ProviderDataIngestionService> logger)
    {
        _showExternalIdRepo = showExternalIdRepo;
        _episodeExternalIdRepo = episodeExternalIdRepo;
        _tvShowRepo = tvShowRepo;
        _episodeRepo = episodeRepo;
        _seasonRepo = seasonRepo;
        _seasonPackRepo = seasonPackRepo;
        _tmdbClient = tmdbClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TvShow> MergeShowAsync(
        DataSource source,
        string providerExternalId,
        string showName,
        ThirdPartyShowIds? thirdPartyIds,
        CancellationToken token)
    {
        // Step 1: Fast path — already imported from this provider
        var existingExtId = await _showExternalIdRepo
            .GetBySourceAndExternalIdAsync(source, providerExternalId, token);

        if (existingExtId != null)
        {
            _logger.LogDebug(
                "Show {ShowName} already imported from {Source} with external ID {ExtId}",
                showName, source, providerExternalId);

            await BackfillAndSaveAsync(existingExtId.TvShow, thirdPartyIds, token);
            return existingExtId.TvShow;
        }

        // Step 2: Match by TvDB ID (most reliable for TV shows)
        TvShow? matched = null;
        if (thirdPartyIds?.TvdbId is > 0)
        {
            matched = await _tvShowRepo
                .GetByTvdbIdAsync(thirdPartyIds.TvdbId.Value, token)
                .FirstOrDefaultAsync(token);

            if (matched != null)
            {
                _logger.LogDebug(
                    "Show {ShowName} matched by TvDB ID {TvdbId} to existing show {MatchedName} (Id={MatchedId})",
                    showName, thirdPartyIds.TvdbId, matched.Name, matched.Id);
            }
        }

        // Step 3: Match by TMDB ID
        if (matched == null && thirdPartyIds?.TmdbId is > 0)
        {
            matched = await _tvShowRepo
                .GetShowsByTmdbIdAsync(thirdPartyIds.TmdbId.Value)
                .FirstOrDefaultAsync(token);

            if (matched != null)
            {
                _logger.LogDebug(
                    "Show {ShowName} matched by TMDB ID {TmdbId} to existing show {MatchedName} (Id={MatchedId})",
                    showName, thirdPartyIds.TmdbId, matched.Name, matched.Id);
            }
        }

        // Step 4: IMDB → TMDB lookup, then match by discovered TMDB ID
        if (matched == null && !string.IsNullOrEmpty(thirdPartyIds?.ImdbId))
        {
            var resolvedTmdbId = await ResolveImdbToTmdbIdAsync(thirdPartyIds.ImdbId, token);
            if (resolvedTmdbId.HasValue)
            {
                matched = await _tvShowRepo
                    .GetShowsByTmdbIdAsync(resolvedTmdbId.Value)
                    .FirstOrDefaultAsync(token);

                // Enrich thirdPartyIds with the discovered TMDB ID for backfill
                thirdPartyIds = thirdPartyIds with { TmdbId = resolvedTmdbId.Value };

                if (matched != null)
                {
                    _logger.LogDebug(
                        "Show {ShowName} matched via IMDB {ImdbId} → TMDB {TmdbId} to existing show {MatchedName} (Id={MatchedId})",
                        showName, thirdPartyIds.ImdbId, resolvedTmdbId, matched.Name, matched.Id);
                }
            }
        }

        // Step 5: Matched — backfill IDs and create the ShowExternalId mapping
        if (matched != null)
        {
            await BackfillAndSaveAsync(matched, thirdPartyIds, token);

            await _showExternalIdRepo.UpsertAsync(new ShowExternalId
            {
                TvShowId = matched.Id,
                Source = source,
                ExternalId = providerExternalId
            }, token);

            return matched;
        }

        // Step 6: No match — create new TvShow
        _logger.LogInformation(
            "Creating new show {ShowName} from {Source} (no match found by TvDB/TMDB/IMDB)",
            showName, source);

        var newShow = new TvShow
        {
            Name = showName,
            Source = source,
            TvdbId = thirdPartyIds?.TvdbId,
            TmdbId = thirdPartyIds?.TmdbId,
            Discovered = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        await _tvShowRepo.InsertShowAsync(newShow, token);

        await _showExternalIdRepo.UpsertAsync(new ShowExternalId
        {
            TvShowId = newShow.Id,
            Source = source,
            ExternalId = providerExternalId
        }, token);

        return newShow;
    }

    /// <inheritdoc />
    public async Task MergeEpisodeSubtitleAsync(
        TvShow show,
        DataSource source,
        int season,
        int episodeNumber,
        string? episodeTitle,
        string? episodeExternalId,
        SubtitleEntity subtitle,
        CancellationToken token)
    {
        // Step 1: Ensure the Season entity exists for this season number
        await EnsureSeasonExistsAsync(show.Id, season, token);

        // Step 2: Build Episode with the Subtitle attached.
        // UpsertEpisodes handles both episode and subtitle merge via IncludeGraph.
        var episode = new Episode
        {
            TvShowId = show.Id,
            TvShow = show,
            Season = season,
            Number = episodeNumber,
            Title = episodeTitle ?? string.Empty,
            Discovered = DateTime.UtcNow,
            Subtitles = [subtitle]
        };

        // Step 3: Upsert Episode + Subtitle
        // Episode key: (TvShowId, Season, Number)
        // Subtitle key: DownloadUri
        await _episodeRepo.UpsertEpisodes([episode], token);

        // Step 4: Upsert EpisodeExternalId if provider-specific ID is available
        if (!string.IsNullOrEmpty(episodeExternalId))
        {
            // Re-fetch the episode to get its database-generated Id
            var upsertedEp = await _episodeRepo
                .GetEpisodeUntrackedAsync(show.Id, season, episodeNumber, token);

            if (upsertedEp != null)
            {
                await _episodeExternalIdRepo.UpsertAsync(new EpisodeExternalId
                {
                    EpisodeId = upsertedEp.Id,
                    Source = source,
                    ExternalId = episodeExternalId
                }, token);
            }
            else
            {
                _logger.LogWarning(
                    "Failed to retrieve episode after upsert for show {ShowId} S{Season:D2}E{Episode:D2}",
                    show.Id, season, episodeNumber);
            }
        }
    }

    /// <inheritdoc />
    public Task IngestSeasonPackAsync(SeasonPackSubtitle seasonPack, CancellationToken token)
    {
        return _seasonPackRepo.BulkUpsertAsync([seasonPack], token);
    }

    /// <summary>
    /// Resolve an IMDB ID to a TMDB ID using the TMDB find endpoint.
    /// Checks TV results first, then movie results.
    /// </summary>
    private async Task<int?> ResolveImdbToTmdbIdAsync(string imdbId, CancellationToken token)
    {
        FindByExternalIdResult? findResult;
        try
        {
            findResult = await _tmdbClient.FindByExternalIdAsync(imdbId, "imdb_id", token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to resolve IMDB ID {ImdbId} via TMDB", imdbId);
            return null;
        }

        if (findResult == null)
        {
            return null;
        }

        if (findResult.TvResults.Count > 0)
        {
            return findResult.TvResults[0].Id;
        }

        if (findResult.MovieResults.Count > 0)
        {
            return findResult.MovieResults[0].Id;
        }

        _logger.LogDebug("No TMDB match found for IMDB ID {ImdbId}", imdbId);
        return null;
    }

    /// <summary>
    /// Backfill missing TvDB/TMDB IDs on an existing show if the provider provides them.
    /// Only saves if changes were made.
    /// </summary>
    private async Task BackfillAndSaveAsync(TvShow show, ThirdPartyShowIds? ids, CancellationToken token)
    {
        if (ids == null)
        {
            return;
        }

        var changed = false;

        if (!show.TvdbId.HasValue && ids.TvdbId is > 0)
        {
            show.TvdbId = ids.TvdbId;
            changed = true;
        }

        if (!show.TmdbId.HasValue && ids.TmdbId is > 0)
        {
            show.TmdbId = ids.TmdbId;
            changed = true;
        }

        if (changed)
        {
            _logger.LogDebug(
                "Backfilled IDs on show {ShowName} (Id={ShowId}): TvdbId={TvdbId}, TmdbId={TmdbId}",
                show.Name, show.Id, show.TvdbId, show.TmdbId);
            await _tvShowRepo.UpdateShowAsync(show, token);
        }
    }

    /// <summary>
    /// Ensure a Season entity exists for the given show and season number.
    /// Creates it via InsertNewSeasonsAsync if missing.
    /// </summary>
    private async Task EnsureSeasonExistsAsync(long showId, int seasonNumber, CancellationToken token)
    {
        var existing = await _seasonRepo.GetSeasonForShowAsync(showId, seasonNumber, token);
        if (existing == null)
        {
            await _seasonRepo.InsertNewSeasonsAsync(showId,
            [
                new Season
                {
                    TvShowId = showId,
                    Number = seasonNumber
                }
            ], token);
        }
    }
}
