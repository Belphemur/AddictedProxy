#region

using System.ComponentModel.DataAnnotations;
using AddictedProxy.Culture.Service;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Model.Dto;
using AddictedProxy.Model.Responses;
using AddictedProxy.Model.Search;
using AddictedProxy.Services.Provider.SeasonPack;
using AddictedProxy.Services.Provider.Subtitle;
using AddictedProxy.Services.Search;
using AddictedProxy.Upstream.Service.Exception;
using AddictedProxy.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SuperSubtitleClient.Service.Exception;

#endregion

namespace AddictedProxy.Controllers.Rest;

/// <summary>
/// Use to interact with the subtitle, finding them, downloading them.
/// </summary>
[ApiController]
[Route("subtitles")]
public class SubtitlesController : Controller
{
    private readonly ICultureParser _cultureParser;
    private readonly ISubtitleProvider _subtitleProvider;
    private readonly ISeasonPackProvider _seasonPackProvider;
    private readonly ISeasonPackSubtitleRepository _seasonPackSubtitleRepository;
    private readonly ISearchSubtitlesService _searchSubtitlesService;
    private readonly ILogger<SubtitlesController> _logger;

    private const string SeasonPackPrefix = "sp_";
    private const string EpisodeSeparator = "_ep_";
    private const string EntrySeparator = "_entry_";

    public SubtitlesController(
        ICultureParser cultureParser,
        ISubtitleProvider subtitleProvider,
        ISeasonPackProvider seasonPackProvider,
        ISeasonPackSubtitleRepository seasonPackSubtitleRepository,
        ISearchSubtitlesService searchSubtitlesService,
        ILogger<SubtitlesController> logger
    )
    {
        _cultureParser = cultureParser;
        _subtitleProvider = subtitleProvider;
        _seasonPackProvider = seasonPackProvider;
        _seasonPackSubtitleRepository = seasonPackSubtitleRepository;
        _searchSubtitlesService = searchSubtitlesService;
        _logger = logger;
    }


    /// <summary>
    /// Download specific subtitle. Supports regular subtitle GUIDs, season pack ZIPs (sp_{uuid}), and single episode extraction from season packs (sp_{uuid}_ep_{N}).
    /// </summary>
    /// <param name="subtitleId">Subtitle identifier: a GUID, sp_{uuid}, or sp_{uuid}_ep_{N}</param>
    /// <param name="token"></param>
    /// <returns></returns>
    [Route("download/{subtitleId}", Name = nameof(Routes.DownloadSubtitle))]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 400, "application/json")]
    [ProducesResponseType(typeof(ErrorResponse), 404, "application/json")]
    [ProducesResponseType(typeof(ErrorResponse), 429)]
    [HttpGet]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 8 * 86400)]
    public async Task<Results<FileStreamHttpResult, NotFound<string>, JsonHttpResult<ErrorResponse>, BadRequest<ErrorResponse>>> Download([FromRoute] string subtitleId, CancellationToken token)
    {
        // Season pack entry: sp_{packUuid}_entry_{entryUuid} — specific file from catalog
        if (subtitleId.StartsWith(SeasonPackPrefix, StringComparison.OrdinalIgnoreCase) && subtitleId.Contains(EntrySeparator, StringComparison.OrdinalIgnoreCase))
        {
            return await DownloadSeasonPackEntryAsync(subtitleId, token);
        }

        // Season pack episode (legacy/uncataloged): sp_{uuid}_ep_{N} — upstream extraction
        if (subtitleId.StartsWith(SeasonPackPrefix, StringComparison.OrdinalIgnoreCase) && subtitleId.Contains(EpisodeSeparator, StringComparison.OrdinalIgnoreCase))
        {
            return await DownloadSeasonPackEpisodeAsync(subtitleId, token);
        }

        // Season pack: sp_{uuid} — full ZIP
        if (subtitleId.StartsWith(SeasonPackPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return await DownloadSeasonPackZipAsync(subtitleId, token);
        }

        // Regular subtitle GUID
        if (!Guid.TryParse(subtitleId, out var guid))
        {
            return TypedResults.BadRequest(new ErrorResponse("Invalid subtitle ID format"));
        }

        return await DownloadRegularSubtitleAsync(guid, token);
    }

    private async Task<Results<FileStreamHttpResult, NotFound<string>, JsonHttpResult<ErrorResponse>, BadRequest<ErrorResponse>>> DownloadRegularSubtitleAsync(Guid subtitleId, CancellationToken token)
    {
        try
        {
            var subtitle = await _subtitleProvider.GetSubtitleFullAsync(subtitleId, token);
            if (subtitle == null)
            {
                return TypedResults.NotFound($"Subtitle ({subtitleId}) couldn't be found");
            }

            var subtitleStream = await _subtitleProvider.GetSubtitleFileAsync(subtitle, token);

            var fileName =
                $"{subtitle.Episode.TvShow.Name.Replace(" ", ".")}.S{subtitle.Episode.Season:D2}E{subtitle.Episode.Number:D2}{(string.IsNullOrWhiteSpace(subtitle.Scene) ? "" : $".{subtitle.Scene}")}{(subtitle.HearingImpaired ? ".hi" : "")}.{(await _cultureParser.FromStringAsync(subtitle.Language, token))?.TwoLetterISOLanguageName.ToLowerInvariant()}.srt";

            return TypedResults.Stream(
                subtitleStream,
                contentType: "text/srt",
                fileDownloadName: fileName,
                lastModified: subtitle.StoredAt,
                entityTag: new EntityTagHeaderValue('"' + $"{subtitle.UniqueId}{(subtitle.StoredAt.HasValue ? "-" + subtitle.StoredAt.Value.Ticks : "")}" + '"')
            );
        }
        catch (DownloadLimitExceededException e)
        {
            return TypedResults.Json(new ErrorResponse(e.Message), statusCode: 429);
        }
        catch (SubtitleFileDeletedException)
        {
            return TypedResults.NotFound("Subtitle was deleted from Addicted");
        }
        catch (SuperSubtitleDownloadException e)
        {
            _logger.LogWarning(e, "Failed to download subtitle {SubtitleId} from SuperSubtitles", subtitleId);
            return TypedResults.NotFound($"Subtitle ({subtitleId}) couldn't be downloaded from SuperSubtitles");
        }
    }

    private async Task<Results<FileStreamHttpResult, NotFound<string>, JsonHttpResult<ErrorResponse>, BadRequest<ErrorResponse>>> DownloadSeasonPackZipAsync(string subtitleId, CancellationToken token)
    {
        var guidStr = subtitleId[SeasonPackPrefix.Length..];
        if (!Guid.TryParse(guidStr, out var packGuid))
        {
            return TypedResults.BadRequest(new ErrorResponse("Invalid season pack ID format"));
        }

        var seasonPack = await _seasonPackProvider.GetByUniqueIdAsync(packGuid, token);
        if (seasonPack == null)
        {
            return TypedResults.NotFound($"Season pack ({packGuid}) couldn't be found");
        }

        var stream = await _seasonPackProvider.GetSeasonPackZipAsync(seasonPack, token);
        var lang = (await _cultureParser.FromStringAsync(seasonPack.Language, token))?.TwoLetterISOLanguageName.ToLowerInvariant() ?? seasonPack.LanguageIsoCode?.ToLowerInvariant() ?? "unknown";
        var fileName = $"{seasonPack.TvShow.Name.Replace(" ", ".")}.S{seasonPack.Season:D2}.{lang}.zip";

        return TypedResults.Stream(
            stream,
            contentType: "application/zip",
            fileDownloadName: fileName,
            lastModified: seasonPack.StoredAt,
            entityTag: new EntityTagHeaderValue('"' + $"{seasonPack.UniqueId}{(seasonPack.StoredAt.HasValue ? "-" + seasonPack.StoredAt.Value.Ticks : "")}" + '"')
        );
    }

    private async Task<Results<FileStreamHttpResult, NotFound<string>, JsonHttpResult<ErrorResponse>, BadRequest<ErrorResponse>>> DownloadSeasonPackEntryAsync(string subtitleId, CancellationToken token)
    {
        var withoutPrefix = subtitleId[SeasonPackPrefix.Length..];
        var entryIndex = withoutPrefix.IndexOf(EntrySeparator, StringComparison.OrdinalIgnoreCase);
        if (entryIndex < 0)
        {
            return TypedResults.BadRequest(new ErrorResponse("Invalid season pack entry ID format"));
        }

        var packGuidStr = withoutPrefix[..entryIndex];
        var entryGuidStr = withoutPrefix[(entryIndex + EntrySeparator.Length)..];

        if (!Guid.TryParse(packGuidStr, out var packGuid))
        {
            return TypedResults.BadRequest(new ErrorResponse("Invalid season pack ID format"));
        }

        if (!Guid.TryParse(entryGuidStr, out var entryGuid))
        {
            return TypedResults.BadRequest(new ErrorResponse("Invalid season pack entry ID format"));
        }

        var seasonPack = await _seasonPackProvider.GetByUniqueIdAsync(packGuid, token);
        if (seasonPack == null)
        {
            return TypedResults.NotFound($"Season pack ({packGuid}) couldn't be found");
        }

        var entry = await _seasonPackProvider.GetEntryByUniqueIdAsync(entryGuid, token);
        if (entry == null || entry.SeasonPackSubtitleId != seasonPack.Id)
        {
            return TypedResults.NotFound($"Entry ({entryGuid}) not found in season pack ({packGuid})");
        }

        var stream = await _seasonPackProvider.GetEntryFileAsync(seasonPack, entry, token);
        var lang = (await _cultureParser.FromStringAsync(seasonPack.Language, token))?.TwoLetterISOLanguageName.ToLowerInvariant() ?? seasonPack.LanguageIsoCode?.ToLowerInvariant() ?? "unknown";
        var fileName = $"{seasonPack.TvShow.Name.Replace(" ", ".")}.S{seasonPack.Season:D2}E{entry.EpisodeNumber:D2}.{lang}.srt";

        return TypedResults.Stream(
            stream,
            contentType: "text/srt",
            fileDownloadName: fileName,
            lastModified: seasonPack.StoredAt,
            entityTag: new EntityTagHeaderValue('"' + $"{entry.UniqueId}{(seasonPack.StoredAt.HasValue ? "-" + seasonPack.StoredAt.Value.Ticks : "")}" + '"')
        );
    }

    private async Task<Results<FileStreamHttpResult, NotFound<string>, JsonHttpResult<ErrorResponse>, BadRequest<ErrorResponse>>> DownloadSeasonPackEpisodeAsync(string subtitleId, CancellationToken token)
    {
        var withoutPrefix = subtitleId[SeasonPackPrefix.Length..];
        var epIndex = withoutPrefix.IndexOf(EpisodeSeparator, StringComparison.OrdinalIgnoreCase);
        if (epIndex < 0)
        {
            return TypedResults.BadRequest(new ErrorResponse("Invalid season pack episode ID format"));
        }

        var guidStr = withoutPrefix[..epIndex];
        var episodeStr = withoutPrefix[(epIndex + EpisodeSeparator.Length)..];

        if (!Guid.TryParse(guidStr, out var packGuid))
        {
            return TypedResults.BadRequest(new ErrorResponse("Invalid season pack ID format"));
        }

        if (!int.TryParse(episodeStr, out var episodeNumber) || episodeNumber <= 0)
        {
            return TypedResults.BadRequest(new ErrorResponse("Invalid episode number in season pack ID"));
        }

        var seasonPack = await _seasonPackProvider.GetByUniqueIdAsync(packGuid, token);
        if (seasonPack == null)
        {
            return TypedResults.NotFound($"Season pack ({packGuid}) couldn't be found");
        }

        try
        {
            var stream = await _seasonPackProvider.GetEpisodeFromUpstreamAsync(seasonPack, episodeNumber, token);
            var lang = (await _cultureParser.FromStringAsync(seasonPack.Language, token))?.TwoLetterISOLanguageName.ToLowerInvariant() ?? seasonPack.LanguageIsoCode?.ToLowerInvariant() ?? "unknown";
            var fileName = $"{seasonPack.TvShow.Name.Replace(" ", ".")}.S{seasonPack.Season:D2}E{episodeNumber:D2}.{lang}.srt";

            return TypedResults.Stream(
                stream,
                contentType: "text/srt",
                fileDownloadName: fileName,
                lastModified: seasonPack.StoredAt,
                entityTag: new EntityTagHeaderValue('"' + $"{seasonPack.UniqueId}{(seasonPack.StoredAt.HasValue ? "-" + seasonPack.StoredAt.Value.Ticks : "")}" + '"')
            );
        }
        catch (EpisodeNotInSeasonPackException)
        {
            return TypedResults.NotFound($"Episode {episodeNumber} not found in season pack ({packGuid})");
        }
    }

    private async Task<Results<Ok<SubtitleSearchResponse>, NotFound<ErrorResponse>, StatusCodeHttpResult>> ProcessSubtitleSearch(
        Results<Ok<TvShow>, NotFound> showResult,
        int episode,
        int season,
        string lang,
        CancellationToken token)
    {
        return await showResult.MatchAsync(
            onOk: async tvShow =>
            {
                var found = await _searchSubtitlesService.FindSubtitlesAsync(new SearchPayload(tvShow, episode, season, lang, null), token);

                return await found.MatchAsync<SubtitleFound, Results<Ok<SubtitleSearchResponse>, NotFound<ErrorResponse>, StatusCodeHttpResult>>(
                    onOk: async subtitleFound =>
                    {
                        var foundMatchingSubtitles = subtitleFound.MatchingSubtitles.Select(
                            subtitle => new SubtitleDto(
                                subtitle,
                                Url.RouteUrl(nameof(Routes.DownloadSubtitle), new Dictionary<string, object> { { "subtitleId", subtitle.UniqueId } }) ??
                                throw new InvalidOperationException("Couldn't find the route for the download subtitle"),
                                subtitleFound.Language
                            )
                        ).ToList();

                        // Fall back to season packs when no episode subtitles found
                        if (foundMatchingSubtitles.Count == 0)
                        {
                            var seasonPackDtos = await GetSeasonPackFallbackSubtitleDtos(tvShow, season, episode, subtitleFound.Language, token);
                            foundMatchingSubtitles.AddRange(seasonPackDtos);
                        }

                        return TypedResults.Ok(new SubtitleSearchResponse(foundMatchingSubtitles, subtitleFound.Episode));
                    },
                    onStatusCode: statusCode =>
                    {
                        // StatusCode 423 (Locked) indicates the resource is temporarily unavailable (refreshing)
                        Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
                        {
                            Public = true,
                            MaxAge = TimeSpan.FromDays(0.5)
                        };
                        return statusCode;
                    }
                );
            },
            onNotFound: () =>
            {
                Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = TimeSpan.FromDays(0.5)
                };
                return TypedResults.NotFound(new ErrorResponse("Couldn't find show"));
            }
        );
    }

    private async Task<IEnumerable<SubtitleDto>> GetSeasonPackFallbackSubtitleDtos(TvShow tvShow, int season, int episode, Culture.Model.Culture language, CancellationToken token)
    {
        var seasonPacks = await _seasonPackSubtitleRepository.GetByShowAndSeasonAsync(tvShow.Id, season, token);
        var isoCode = language.TwoLetterISOLanguageName;

        var result = new List<SubtitleDto>();

        foreach (var pack in seasonPacks.Where(sp => string.Equals(sp.LanguageIsoCode, isoCode, StringComparison.OrdinalIgnoreCase)))
        {
            var entriesForEpisode = pack.Entries.Where(e => e.EpisodeNumber == episode).ToList();

            if (entriesForEpisode.Count > 0)
            {
                // Cataloged: produce one DTO per entry (e.g., regular + dubtitle)
                foreach (var entry in entriesForEpisode)
                {
                    var spSubtitleId = $"{SeasonPackPrefix}{pack.UniqueId}{EntrySeparator}{entry.UniqueId}";
                    var downloadUri = Url.RouteUrl(nameof(Routes.DownloadSubtitle), new Dictionary<string, object> { { "subtitleId", spSubtitleId } })
                                      ?? throw new InvalidOperationException("Couldn't find the route for the download subtitle");
                    result.Add(new SubtitleDto(pack, entry, downloadUri, language, episode));
                }
            }
            else if (pack.Entries.Count == 0)
            {
                // Not cataloged yet — graceful degradation: offer the pack-level DTO
                var spSubtitleId = $"{SeasonPackPrefix}{pack.UniqueId}{EpisodeSeparator}{episode}";
                var downloadUri = Url.RouteUrl(nameof(Routes.DownloadSubtitle), new Dictionary<string, object> { { "subtitleId", spSubtitleId } })
                                  ?? throw new InvalidOperationException("Couldn't find the route for the download subtitle");
                result.Add(new SubtitleDto(pack, downloadUri, language, episode));
            }
            // else: cataloged but no entry for this episode — skip
        }

        return result;
    }


    /// <summary>
    /// Get subtitles for an episode of a given show in the wanted language
    /// </summary>
    /// <param name="language">Language to search for</param>
    /// <param name="episode">Episode number to look for</param>
    /// <param name="token"></param>
    /// <param name="showUniqueId">Unique ID of the show, you can get it from Shows::Search</param>
    /// <param name="season">Season number to look for</param>
    /// <returns></returns>
    /// <response code="200">Returns the matching subtitles</response>
    /// <response code="404">Couldn't find the show or its season/episode</response>
    /// <response code="429">Reached the rate limiting of the endpoint</response>
    /// <response code="423">Refreshing the show, currently don't have data, try again later</response>
    /// <remarks>
    /// Start by using the /shows/search/SHOW_NAME to find the showUniqueId of the show you're interested in.
    ///
    /// Then use the subtitles/get/ endpoint to get subtitle for the episode you want.
    /// </remarks>
    [Route("get/{showUniqueId:guid}/{season:int:min(0)}/{episode:int:min(0)}/{**language:minlength(2)}")]
    [HttpGet]
    [ProducesResponseType(typeof(SubtitleSearchResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(string), 429)]
    [Produces("application/json")]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 7200)]
    public async Task<Results<Ok<SubtitleSearchResponse>, NotFound<ErrorResponse>, StatusCodeHttpResult>> GetSubtitles(string language, Guid showUniqueId, int season, int episode, CancellationToken token)
    {
        var findShow = await _searchSubtitlesService.GetByShowUniqueIdAsync(showUniqueId, token);
        return await ProcessSubtitleSearch(findShow, episode, season, language, token);
    }

    /// <summary>
    /// Used for different Media Center/Subtitle searchers
    /// </summary>
    public class SubtitleQueryRequest
    {
        public SubtitleQueryRequest(string show, int episode, int season, string languageIso, string? fileName)
        {
            Show = show;
            Episode = episode;
            Season = season;
            FileName = fileName;
            LanguageISO = languageIso;
        }

        /// <summary>
        /// Name of the show
        /// </summary>
        /// <example>NCIS</example>

        [Required]
        public string Show { get; }

        /// <summary>
        /// Episode number
        /// </summary>
        /// <example>1</example>
        [Required]
        public int Episode { get; }

        /// <summary>
        /// Season number
        /// </summary>
        /// <example>1</example>
        [Required]
        public int Season { get; }

        /// <summary>
        /// Name of the file for which you want subtitle, it help find a version of the subtitle that matches it
        /// </summary>
        /// <example>NCIS.S01E01.HDTV.mkv</example>
        /// <remarks>Optional, only if you want to match by version of subtitle</remarks>
        public string? FileName { get; }

        /// <summary>
        ///     3 or 2 letter code of the language
        /// </summary>
        /// <example>en</example>
        [Required]
        public string LanguageISO { get; }
    }
}