using AddictedProxy.Database.Model;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Model.Dto;
using AddictedProxy.Services.Provider.Shows;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Net.Http.Headers;
using TvMovieDatabaseClient.Service;
using TvMovieDatabaseClient.Service.Model;

namespace AddictedProxy.Controllers.Rest;

[Route("media")]
public class MediaController : Controller
{
    private readonly IShowRefresher _showRefresher;
    private readonly ITMDBClient _tmdbClient;
    private readonly ITvShowRepository _tvShowRepository;
    private readonly ILogger<MediaController> _logger;

    public MediaController(IShowRefresher showRefresher, ITMDBClient tmdbClient, ITvShowRepository tvShowRepository, ILogger<MediaController> logger)
    {
        _showRefresher = showRefresher;
        _tmdbClient = tmdbClient;
        _tvShowRepository = tvShowRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get the trending media of the last week
    /// </summary>
    /// <param name="max"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Route("trending/{max:range(0,50)}")]
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

        var tmdbShows = await _tmdbClient.GetTrendingTvAsync(TimeWindowEnum.week, cancellationToken).Take(max).ToDictionaryAsync(searchResult => searchResult.Id, cancellationToken);
        var shows = _tvShowRepository.GetShowsByTmdbIdAsync(tmdbShows.Keys.ToArray());
        var genres = (await _tmdbClient.GetTvGenresAsync(cancellationToken)).ToDictionary(genre => genre.Id, genre => genre.Name);

        var result = shows.Select(show =>
        {
            var showDto = new ShowDto(show);
            var showDetails = tmdbShows[show.TmdbId!.Value];
            var genreNames = showDetails.GenreIds.Select(i => genres[i]).ToArray();
            
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
            details = UpdatePathAndVoteDetailsDto(details);

            return new MediaDetailsDto(showDto, details);
        });
        return TypedResults.Ok(result);
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

        MediaDetailsDto.DetailsDto? detailsDto = null;
        if (show.TmdbId.HasValue)
        {
            switch (show.Type)
            {
                case ShowType.Show:
                {
                    var showDetails = await _tmdbClient.GetShowDetailsByIdAsync(show.TmdbId.Value, cancellationToken);
                    if (showDetails == null)
                    {
                        _logger.LogWarning("Couldn't find show details for TMDB {id}", show.TmdbId);
                        break;
                    };
                    int? year = DateTime.TryParse(showDetails.FirstAirDate, out var releaseDate) ? releaseDate.Year : null;

                    detailsDto = new MediaDetailsDto.DetailsDto(showDetails.PosterPath,
                        showDetails.Overview,
                        showDetails.OriginalName,
                        (MediaDetailsDto.MediaType)show.Type,
                        showDetails.BackdropPath,
                        showDetails.VoteAverage,
                        showDetails.Genres.Select(genre => genre.Name).ToArray(),
                        showDetails.Tagline,
                        year,
                        showDetails.Name);
                }
                    break;
                case ShowType.Movie:
                {
                    var movieDetails = await _tmdbClient.GetMovieDetailsByIdAsync(show.TmdbId.Value, cancellationToken);
                    if (movieDetails == null)
                    {
                        _logger.LogWarning("Couldn't find movie details for TMDB {id}", show.TmdbId);
                        break;
                    }

                    int? year = DateTime.TryParse(movieDetails.ReleaseDate, out var releaseDate) ? releaseDate.Year : null;

                    detailsDto = new MediaDetailsDto.DetailsDto(movieDetails.PosterPath,
                        movieDetails.Overview,
                        movieDetails.OriginalTitle,
                        (MediaDetailsDto.MediaType)show.Type,
                        movieDetails.BackdropPath,
                        movieDetails.VoteAverage,
                        movieDetails.Genres.Select(genre => genre.Name).ToArray(),
                        movieDetails.Tagline,
                        year,
                        movieDetails.Title);
                }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            detailsDto = UpdatePathAndVoteDetailsDto(detailsDto);
        }

        return TypedResults.Ok(new MediaDetailsDto(new ShowDto(show), detailsDto));
    }

    private static MediaDetailsDto.DetailsDto? UpdatePathAndVoteDetailsDto(MediaDetailsDto.DetailsDto? detailsDto)
    {
        if (detailsDto == null)
        {
            return null;
        }
        detailsDto = detailsDto with
        {
            PosterPath = $"https://image.tmdb.org/t/p/original{detailsDto.PosterPath}",
            BackdropPath = $"https://image.tmdb.org/t/p/original{detailsDto.BackdropPath}",
            VoteAverage = Math.Round(detailsDto.VoteAverage, 1)
        };
        return detailsDto;
    }
}