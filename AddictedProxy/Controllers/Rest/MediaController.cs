using AddictedProxy.Caching.Extensions;
using AddictedProxy.Database.Model;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Model.Dto;
using AddictedProxy.Services.Provider.Shows;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Net.Http.Headers;
using System.Collections.Frozen;
using AddictedProxy.Culture.Service;
using AddictedProxy.Services.Details;
using AddictedProxy.Services.Provider.Shows.Jobs;
using Hangfire;
using TvMovieDatabaseClient.Service;
using TvMovieDatabaseClient.Service.Model;
using DistributedCacheExtensions = AddictedProxy.Caching.Extensions.DistributedCacheExtensions;

namespace AddictedProxy.Controllers.Rest;

[Route("media")]
public class MediaController : Controller
{
    private readonly IShowRefresher _showRefresher;
    private readonly ITMDBClient _tmdbClient;
    private readonly ITvShowRepository _tvShowRepository;
    private readonly IDistributedCache _distributedCache;
    private readonly IMediaDetailsService _mediaDetailsService;
    private readonly IEpisodeRepository _episodeRepository;
    private readonly ICultureParser _cultureParser;
    private readonly LinkGenerator _generator;

    public MediaController(IShowRefresher showRefresher, ITMDBClient tmdbClient, ITvShowRepository tvShowRepository, IDistributedCache distributedCache,
        IMediaDetailsService mediaDetailsService,
        IEpisodeRepository episodeRepository, ICultureParser cultureParser, LinkGenerator generator)
    {
        _showRefresher = showRefresher;
        _tmdbClient = tmdbClient;
        _tvShowRepository = tvShowRepository;
        _distributedCache = distributedCache;
        _mediaDetailsService = mediaDetailsService;
        _episodeRepository = episodeRepository;
        _cultureParser = cultureParser;
        _generator = generator;
    }

    /// <summary>
    /// Get the trending media of the last week
    /// </summary>
    /// <param name="max"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Route("trending/{max:range(1,50)}")]
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IAsyncEnumerable<MediaDetailsDto>), 200)]
    [ProducesResponseType(typeof(string), 429)]
    public async Task<Ok<IAsyncEnumerable<MediaDetailsDto>>> GetTrendingTvShowsAsync([FromRoute] int max, CancellationToken cancellationToken)
    {
        Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
        {
            Public = true,
            MaxAge = TimeSpan.FromDays(1)
        };

        var genres = await GetGenresCachedAsync(cancellationToken);

        var trendingTvShows = _tmdbClient.GetTrendingTvAsync(TimeWindowEnum.week, cancellationToken)
            .SelectAwait(async searchResult => (ShowDetails: searchResult, MatchedShows: await _tvShowRepository.GetShowsByTmdbIdAsync(searchResult.Id).ToArrayAsync(cancellationToken: cancellationToken)))
            .Where(results => results.MatchedShows.Length > 0)
            .Select(results => (results.ShowDetails, MatchedShow: results.MatchedShows.MaxBy(show => show.Seasons.Count)!))
            .Take(max)
            .Select(result =>
            {
                var show = result.MatchedShow;
                var showDetails = result.ShowDetails;
                var showDto = new ShowDto(show);
                var genreNames = showDetails.GenreIds.Select(i =>
                    {
                        genres.TryGetValue(i, out var genreName);
                        return genreName;
                    })
                    .Where(s => s != null)
                    .Cast<string>()
                    .ToArray();

                int? year = DateTime.TryParse(showDetails.FirstAirDate, out var releaseDate) ? releaseDate.Year : null;
                var details = new MediaDetailsDto.DetailsDto(showDetails.PosterPath,
                    showDetails.Overview,
                    showDetails.OriginalName,
                    (MediaDetailsDto.MediaType)show.Type,
                    showDetails.BackdropPath,
                    showDetails.VoteAverage,
                    genreNames,
                    "",
                    year,
                    showDetails.Name);
                details = _mediaDetailsService.UpdatePathAndVoteDetailsDto(details);

                return new MediaDetailsDto(showDto, details);
            });

        return TypedResults.Ok(trendingTvShows);
    }

    private Task<Dictionary<int, string>> GetGenresCachedAsync(CancellationToken cancellationToken)
    {
        return _distributedCache.GetSertAsync($"tmdb-genre", async () =>
        {
            return new DistributedCacheExtensions.CacheData<Dictionary<int, string>>(
                (await _tmdbClient.GetTvGenresAsync(cancellationToken)).ToDictionary(genre => genre.Id, genre => genre.Name),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(1)
                });
        });
    }


    /// <summary>
    /// Get the details of a specific show
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("{showId:guid}/details", Name = nameof(Routes.MediaDetails))]
    [HttpGet]
    [ProducesResponseType(typeof(MediaDetailsDto), 200)]
    [ProducesResponseType(typeof(string), 429)]
    [ProducesResponseType(404)]
    [Produces("application/json")]
    public async Task<Results<Ok<MediaDetailsDto>, NotFound>> GetDetailsById([FromRoute] Guid showId, CancellationToken cancellationToken)
    {
        Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
        {
            Public = true,
            MaxAge = TimeSpan.FromDays(1)
        };

        var show = await _showRefresher.GetShowByGuidAsync(showId, cancellationToken);
        if (show == null)
        {
            Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
            {
                Public = true,
                MaxAge = TimeSpan.FromDays(0.5)
            };
            return TypedResults.NotFound();
        }

        var detailsDto = await _mediaDetailsService.GetDetailsDtoCachedAsync(show, cancellationToken);

        return TypedResults.Ok(new MediaDetailsDto(new ShowDto(show), detailsDto));
    }

    /// <summary>
    /// Get the show details with the last season and episodes
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="language"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [Route("{showId:guid}/episodes/{**language}")]
    [HttpGet]
    [ProducesResponseType(typeof(MediaDetailsWithEpisodeAndSubtitlesDto), 200)]
    [ProducesResponseType(typeof(string), 429)]
    [ProducesResponseType(404)]
    [Produces("application/json")]
    public async Task<Results<Ok<MediaDetailsWithEpisodeAndSubtitlesDto>, NotFound, BadRequest<string>>> GetShowDetails(Guid showId, string language, CancellationToken cancellationToken)
    {
        Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
        {
            Public = true,
            MaxAge = TimeSpan.FromDays(0.5)
        };

        var searchLanguage = await _cultureParser.FromStringAsync(language, cancellationToken);
        if (searchLanguage == null)
        {
            return TypedResults.BadRequest("Invalid language");
        }

        var show = await _showRefresher.GetShowByGuidAsync(showId, cancellationToken);
        if (show == null)
        {
            return TypedResults.NotFound();
        }

        var detailsTask = _mediaDetailsService.GetDetailsDtoCachedAsync(show, cancellationToken);

        var lastSeason = show.Seasons.OrderBy(season => season.Number).LastOrDefault();
        if (lastSeason == null)
        {
            BackgroundJob.Enqueue<RefreshSingleShowJob>(showJob => showJob.ExecuteAsync(show.Id, null, CancellationToken.None));
            return TypedResults.Ok(new MediaDetailsWithEpisodeAndSubtitlesDto(new MediaDetailsDto(new ShowDto(show), await detailsTask), Array.Empty<EpisodeWithSubtitlesDto>().ToAsyncEnumerable(), null));
        }

        var episodes = _episodeRepository.GetSeasonEpisodesByLangUntrackedAsync(show.Id, searchLanguage, lastSeason.Number)
            .Select(episode =>
            {
                var subs = episode
                    .Subtitles
                    .Select(
                        subtitle =>
                            new SubtitleDto(subtitle,
                                _generator.GetUriByRouteValues(HttpContext, nameof(Routes.DownloadSubtitle), new { subtitleId = subtitle.UniqueId }) ??
                                throw new InvalidOperationException("Couldn't find the route for the download subtitle"),
                                searchLanguage)
                    );
                return new EpisodeWithSubtitlesDto(episode, subs);
            });
        return TypedResults.Ok(new MediaDetailsWithEpisodeAndSubtitlesDto(new MediaDetailsDto(new ShowDto(show), await detailsTask), episodes, lastSeason.Number));
    }
}