#region

using System.Globalization;
using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Services.Culture;
using AddictedProxy.Services.Provider.Subtitle;
using AddictedProxy.Services.Saver;
using AddictedProxy.Upstream.Service;
using AddictedProxy.Upstream.Service.Exception;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

#endregion

namespace AddictedProxy.Controllers;

[ApiController]
[Route("addic7ed")]
public class Addic7ed : Controller
{
    private readonly IAddic7edClient _client;
    private readonly ICredentialsService _credentialsService;
    private readonly IShowProvider _showProvider;
    private readonly ISubtitleProvider _subtitleProvider;
    private readonly CultureParser _cultureParser;
    private readonly IAddic7edDownloader _downloader;
    private readonly IEpisodeRepository _episodeRepository;
    private readonly ISeasonRepository _seasonRepository;
    private readonly TimeSpan _timeBetweenChecks;
    private readonly ITvShowRepository _tvShowRepository;

    public Addic7ed(IAddic7edClient client,
                    IAddic7edDownloader downloader,
                    ITvShowRepository tvShowRepository,
                    ISeasonRepository seasonRepository,
                    IEpisodeRepository episodeRepository,
                    CultureParser cultureParser,
                    ICredentialsService credentialsService,
                    IShowProvider showProvider,
                    ISubtitleProvider subtitleProvider)
    {
        _client = client;
        _downloader = downloader;
        _tvShowRepository = tvShowRepository;
        _seasonRepository = seasonRepository;
        _episodeRepository = episodeRepository;
        _cultureParser = cultureParser;
        _credentialsService = credentialsService;
        _showProvider = showProvider;
        _subtitleProvider = subtitleProvider;
        _timeBetweenChecks = TimeSpan.FromHours(1);
    }


    /// <summary>
    /// Download specific subtitle
    /// </summary>
    /// <param name="subtitleId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [Route("download/{subtitleId:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 400, "application/json")]
    [HttpPost]
    public async Task<IActionResult> Download([FromRoute] Guid subtitleId, CancellationToken token)
    {
        try
        {
            var subtitleStream = await _subtitleProvider.GetSubtitleFileStreamAsync(subtitleId, token);
            if (subtitleStream == null)
            {
                return NotFound($"Subtitle ({subtitleId}) couldn't be found");
            }

            return new FileStreamResult(subtitleStream, new MediaTypeHeaderValue("text/srt"));
        }
        catch (DownloadLimitExceededException e)
        {
            return BadRequest(new ErrorResponse(e.Message));
        }
    }

    /// <summary>
    /// Search for subtitle of a specific episode of a show
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [Route("search")]
    [HttpPost]
    [ProducesResponseType(typeof(SearchResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [Produces("application/json")]
    public async Task<IActionResult> Search([FromBody] SearchRequest request, CancellationToken token)
    {
        var show = await _showProvider.FindShowsAsync(request.Show, token).FirstOrDefaultAsync(token);
        if (show == null)
        {
            return NotFound(new { Error = $"Couldn't find the show {request.Show}" });
        }

        var season = show.Seasons.FirstOrDefault(season => season.Number == request.Season);

        await using var credentials = await _credentialsService.GetLeastUsedCredsAsync(token);


        if (season == null && (show.LastSeasonRefreshed == null || DateTime.UtcNow - show.LastSeasonRefreshed >= _timeBetweenChecks))
        {
            var maxSeason = show.Seasons.Any() ? show.Seasons.Max(s => s.Number) : 0;
            if (show.Seasons.Any() && request.Season - maxSeason > 1)
            {
                return NotFound(new ErrorResponse($"{request.Season} is too far in the future."));
            }

            var seasons = (await _client.GetSeasonsAsync(credentials.AddictedUserCredentials, show, token)).ToArray();
            await _seasonRepository.UpsertSeason(seasons, token);
            show.LastSeasonRefreshed = DateTime.UtcNow;
            await _tvShowRepository.UpdateShow(show, token);
            season = await _seasonRepository.GetSeasonForShow(show.Id, request.Season, token);
        }

        if (season == null)
        {
            return NotFound(new ErrorResponse($"Couldn't find Season S{request.Season} for {show.Name}"));
        }

        var episode = await _episodeRepository.GetEpisodeUntrackedAsync(show.Id, season.Number, request.Episode, token);

        var episodesRefreshed = season.LastRefreshed != null && DateTime.UtcNow - season.LastRefreshed <= _timeBetweenChecks;
        if (episode == null && !episodesRefreshed)
        {
            episode = await RefreshSubtitlesAsync(credentials.AddictedUserCredentials, show, season, request.Episode, token);
            episodesRefreshed = true;
        }

        if (episode == null)
        {
            return NotFound(new ErrorResponse($"Couldn't find episode S{season.Number}E{request.Episode} for {show.Name}"));
        }

        var matchingSubtitles = FindMatchingSubtitles(request, episode);

        var latestDiscovered = episode.Subtitles.Max(subtitle => subtitle.Discovered);

        if (matchingSubtitles.Any() || episodesRefreshed || DateTime.UtcNow - latestDiscovered > TimeSpan.FromDays(180))
        {
            return Ok(new SearchResponse(episode: new SearchResponse.EpisodeDto(episode), matchingSubtitles: matchingSubtitles));
        }

        episode = await RefreshSubtitlesAsync(credentials.AddictedUserCredentials, show, season, request.Episode, token);
        matchingSubtitles = FindMatchingSubtitles(request, episode!);


        return Ok(new SearchResponse(episode: new SearchResponse.EpisodeDto(episode!), matchingSubtitles: matchingSubtitles));
    }

    private SearchResponse.SubtitleDto[] FindMatchingSubtitles(SearchRequest request, Episode episode)
    {
        var searchLanguage = _cultureParser.FromString(request.LanguageISO);
        return episode.Subtitles
                      .Where(subtitle => Equals(_cultureParser.FromString(subtitle.Language), searchLanguage))
                      .Where(subtitle => { return subtitle.Scene.ToLowerInvariant().Split('+', '.', '-').Any(version => request.FileName.ToLowerInvariant().Contains(version)); })
                      .Select(subtitle => new SearchResponse.SubtitleDto(subtitle, searchLanguage))
                      .ToArray();
    }

    private async Task<Episode?> RefreshSubtitlesAsync(AddictedUserCredentials credentials, TvShow show, Season season, int episodeNumber, CancellationToken token)
    {
        var episodes = await _client.GetEpisodesAsync(credentials, show, season.Number, token);
        await _episodeRepository.UpsertEpisodes(episodes, token);
        season.LastRefreshed = DateTime.UtcNow;
        await _seasonRepository.UpdateSeasonAsync(season, token);
        return await _episodeRepository.GetEpisodeUntrackedAsync(show.Id, season.Number, episodeNumber, token);
    }

    public record ErrorResponse(string Error);

    public class SearchRequest
    {
        public SearchRequest(string show, int episode, int season, string fileName, string languageIso)
        {
            Show = show;
            Episode = episode;
            Season = season;
            FileName = fileName;
            LanguageISO = languageIso;
        }

        public string Show { get; }
        public int Episode { get; }
        public int Season { get; }
        public string FileName { get; }

        /// <summary>
        ///     3 letter code of the language
        /// </summary>
        public string LanguageISO { get; }
    }

    public class SearchResponse
    {
        public SearchResponse(IEnumerable<SubtitleDto> matchingSubtitles, EpisodeDto episode)
        {
            MatchingSubtitles = matchingSubtitles;
            Episode = episode;
        }

        /// <summary>
        /// Matching subtitle for the filename and language
        /// </summary>
        public IEnumerable<SubtitleDto> MatchingSubtitles { get; }

        /// <summary>
        /// Information about the episode
        /// </summary>
        public EpisodeDto Episode { get; }

        public class SubtitleDto
        {
            public SubtitleDto(Subtitle subtitle, CultureInfo? language)
            {
                Version = subtitle.Scene;
                Completed = subtitle.Completed;
                HearingImpaired = subtitle.HearingImpaired;
                HD = subtitle.HD;
                Corrected = subtitle.Completed;
                DownloadUri = $"/download/{subtitle.UniqueId}";
                Language = language?.EnglishName ?? "Unknown";
                Discovered = subtitle.Discovered;
            }

            public string Version { get; }
            public bool Completed { get; }
            public bool HearingImpaired { get; }
            public bool Corrected { get; }
            public bool HD { get; }
            public string DownloadUri { get; }
            public string Language { get; }

            /// <summary>
            ///     When was the subtitle discovered
            /// </summary>
            public DateTime Discovered { get; }
        }

        /// <summary>
        /// Episode information
        /// </summary>
        public class EpisodeDto
        {
            public EpisodeDto(Episode episode)
            {
                Season = episode.Season;
                Number = episode.Number;
                Title = episode.Title;
                Discovered = episode.Discovered;
                Show = episode.TvShow.Name;
            }

            /// <summary>
            /// Season of the episode
            /// </summary>
            public int Season { get; }

            /// <summary>
            /// Number of the episode
            /// </summary>
            public int Number { get; }

            /// <summary>
            /// Title of the episode
            /// </summary>
            public string Title { get; }

            /// <summary>
            /// For which show
            /// </summary>
            public string Show { get; }

            /// <summary>
            ///     When was the subtitle discovered
            /// </summary>
            public DateTime Discovered { get; }
        }
    }
}