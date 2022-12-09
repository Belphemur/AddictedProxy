﻿#region

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using AddictedProxy.Caching.OutputCache.Configuration;
using AddictedProxy.Culture.Service;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Model.Dto;
using AddictedProxy.Model.Responses;
using AddictedProxy.Model.Search;
using AddictedProxy.Services.Provider.Subtitle;
using AddictedProxy.Services.Search;
using AddictedProxy.Upstream.Service.Exception;
using AddictedProxy.Utils;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Net.Http.Headers;

#endregion

namespace AddictedProxy.Controllers.Rest;

[ApiController]
[Route("subtitles")]
public class SubtitlesController : Controller
{
    private readonly CultureParser _cultureParser;
    private readonly ISubtitleProvider _subtitleProvider;
    private readonly ISearchSubtitlesService _searchSubtitlesService;
    private readonly Regex _searchPattern = new(@"(?<show>.+)S(?<season>\d+)E(?<episode>\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public SubtitlesController(
        CultureParser cultureParser,
        ISubtitleProvider subtitleProvider,
        ISearchSubtitlesService searchSubtitlesService
    )
    {
        _cultureParser = cultureParser;
        _subtitleProvider = subtitleProvider;
        _searchSubtitlesService = searchSubtitlesService;
    }


    /// <summary>
    /// Download specific subtitle
    /// </summary>
    /// <param name="subtitleId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [Route("download/{subtitleId:guid}", Name = nameof(Routes.DownloadSubtitle))]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 400, "application/json")]
    [ProducesResponseType(typeof(ErrorResponse), 429)]
    [HttpGet]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 8 * 86400)]
    public async Task<IActionResult> Download([FromRoute] Guid subtitleId, CancellationToken token)
    {
        try
        {
            var subtitle = await _subtitleProvider.GetSubtitleFullAsync(subtitleId, token);
            if (subtitle == null)
            {
                return NotFound($"Subtitle ({subtitleId}) couldn't be found");
            }

            var subtitleStream = await _subtitleProvider.GetSubtitleFileAsync(subtitle, token);

            var fileName =
                $"{subtitle.Episode.TvShow.Name.Replace(" ", ".")}.S{subtitle.Episode.Season:D2}E{subtitle.Episode.Number:D2}.{(await _cultureParser.FromStringAsync(subtitle.Language, token))?.TwoLetterISOLanguageName.ToLowerInvariant()}{(subtitle.HearingImpaired ? ".hi" : "")}.srt";
            return new FileStreamResult(subtitleStream, new MediaTypeHeaderValue("text/srt"))
            {
                EntityTag = new EntityTagHeaderValue('"' + $"{subtitle.UniqueId}{(subtitle.StoredAt.HasValue ? "-" + subtitle.StoredAt.Value.Ticks : "")}" + '"'),
                LastModified = subtitle.StoredAt,
                FileDownloadName = fileName
            };
        }
        catch (DownloadLimitExceededException e)
        {
            return StatusCode(429, new ErrorResponse(e.Message));
        }
    }

    /// <summary>
    /// Search for a specific episode
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <response code="200">Returns the matching subtitles</response>
    /// <response code="404">Couldn't find the show or its season/episode</response>
    /// <response code="400">Doesn't follow the right format for the search: Show S00E00</response>
    /// <response code="429">Reached the rate limiting of the endpoint</response>
    /// <response code="423">Refreshing the show, currently don't have data, try again later</response>
    [Route("search")]
    [HttpPost]
    [ProducesResponseType(typeof(SubtitleSearchResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 423)]
    [ProducesResponseType(typeof(WrongFormatResponse), 400)]
    [ProducesResponseType(typeof(string), 429)]
    [Produces("application/json")]
    [OutputCache(PolicyName = nameof(PolicyEnum.Shows))]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 7200)]
    [Obsolete("Use " + nameof(GetSubtitles))]
    public async Task<ActionResult<SubtitleSearchResponse>> Search([FromBody] SearchRequest request, CancellationToken token)
    {
        var match = _searchPattern.Match(request.Search);
        if (!match.Success)
        {
            return BadRequest(new WrongFormatResponse("The search doesn't follow the wanted format. Example: Wellington S01E01", request.Search));
        }

        var show = match.Groups["show"].Value.Trim().Replace(".", " ");
        var episode = int.Parse(match.Groups["episode"].Value);
        var season = int.Parse(match.Groups["season"].Value);
        var lang = request.Language;

        var findShow = await _searchSubtitlesService.FindShowAsync(show, token);

        return await SearchSubtitles(findShow, episode, season, lang, token);
    }


    private async Task<ActionResult<SubtitleSearchResponse>> SearchSubtitles(Result<TvShow> showResult, int episode, int season, string lang, CancellationToken token)
    {
        if (!showResult.IsSuccess)
        {
            Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
            {
                Public = true,
                MaxAge = TimeSpan.FromDays(0.5)
            };
        }

        var response =
            await showResult
                  .MapAsync(tvShow => _searchSubtitlesService.FindSubtitlesAsync(new SearchPayload(tvShow, episode, season, lang, null), token))
                  .MapAsync(found =>
                  {
                      var foundMatchingSubtitles = found.MatchingSubtitles.Select(
                          subtitle => new SubtitleDto(
                              subtitle,
                              Url.RouteUrl(nameof(Routes.DownloadSubtitle), new Dictionary<string, object> { { "subtitleId", subtitle.UniqueId } }) ??
                              throw new InvalidOperationException("Couldn't find the route for the download subtitle"),
                              found.Language
                          )
                      );
                      return new SubtitleSearchResponse(foundMatchingSubtitles, found.Episode);
                  });

        return response.ToActionResult(this);
    }

    /// <summary>
    /// Find specific episode (same as search but easily cacheable)
    /// </summary>
    /// <param name="language">Language to search for</param>
    /// <param name="episode">Episode number to look for</param>
    /// <param name="token"></param>
    /// <param name="show">Name of the show</param>
    /// <param name="season">Season number to look for</param>
    /// <returns></returns>
    /// <response code="200">Returns the matching subtitles</response>
    /// <response code="404">Couldn't find the show or its season/episode</response>
    /// <response code="400">Doesn't follow the right format for the search: Show S00E00</response>
    /// <response code="429">Reached the rate limiting of the endpoint</response>
    /// <response code="423">Refreshing the show, currently don't have data, try again later</response>
    [Route("find/{language}/{show}/{season:int:min(0)}/{episode:int:min(0)}")]
    [HttpGet]
    [ProducesResponseType(typeof(SubtitleSearchResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 423)]
    [ProducesResponseType(typeof(WrongFormatResponse), 400)]
    [ProducesResponseType(typeof(string), 429)]
    [Produces("application/json")]
    [OutputCache(PolicyName = nameof(PolicyEnum.Shows))]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 7200)]
    [Obsolete("Use " + nameof(GetSubtitles))]
    public async Task<ActionResult<SubtitleSearchResponse>> Find(string language, string show, int season, int episode, CancellationToken token)
    {
        var findShow = await _searchSubtitlesService.FindShowAsync(show, token);

        return await SearchSubtitles(findShow, episode, season, language, token);
    }

    /// <summary>
    /// Get subtitles for an episode of a given show in the wanted language
    /// </summary>
    /// <param name="language">Language to search for</param>
    /// <param name="episode">Episode number to look for</param>
    /// <param name="token"></param>
    /// <param name="showUniqueId">Name of the show</param>
    /// <param name="season">Season number to look for</param>
    /// <returns></returns>
    /// <response code="200">Returns the matching subtitles</response>
    /// <response code="404">Couldn't find the show or its season/episode</response>
    /// <response code="400">Doesn't follow the right format for the search: Show S00E00</response>
    /// <response code="429">Reached the rate limiting of the endpoint</response>
    /// <response code="423">Refreshing the show, currently don't have data, try again later</response>
    [Route("get/{showUniqueId:guid}/{season:int:min(0)}/{episode:int:min(0)}/{language:alpha}")]
    [HttpGet]
    [ProducesResponseType(typeof(SubtitleSearchResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(WrongFormatResponse), 400)]
    [ProducesResponseType(typeof(string), 429)]
    [Produces("application/json")]
    [OutputCache(PolicyName = nameof(PolicyEnum.Shows))]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 7200)]
    public async Task<ActionResult<SubtitleSearchResponse>> GetSubtitles(string language, Guid showUniqueId, int season, int episode, CancellationToken token)
    {
        var findShow = await _searchSubtitlesService.GetByShowUniqueIdAsync(showUniqueId, token);
        return await SearchSubtitles(findShow, episode, season, language, token);
    }

    /// <summary>
    /// Returned when the search wasn't formatted properly
    /// </summary>
    /// <param name="Error"></param>
    public record WrongFormatResponse(string Error, string Search) : ErrorResponse(Error);

    /// <summary>
    /// Use for the website to provide easy search for the user
    /// </summary>
    /// <param name="Search"></param>
    /// <param name="Language"></param>
    public record SearchRequest(string Search, string Language)
    {
        /// <summary>
        /// Search for specific subtitle
        /// </summary>
        /// <example>Wellington Paranormal S01E05</example>
        public string Search { get; init; } = Search;

        /// <summary>
        /// Language of the subtitle
        /// </summary>
        /// <example>English</example>
        public string Language { get; init; } = Language;
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