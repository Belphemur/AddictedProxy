using AddictedProxy.Database.Model;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Model.Dto;
using Microsoft.Extensions.Caching.Distributed;
using TvMovieDatabaseClient.Model.Common;
using TvMovieDatabaseClient.Service;
using DistributedCacheExtensions = AddictedProxy.Caching.Extensions.DistributedCacheExtensions;

namespace AddictedProxy.Services.Details;

public class ShowDetailsService : IShowDetailsService
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<ShowDetailsService> _logger;
    private readonly ITMDBClient _tmdbClient;


    public ShowDetailsService(IDistributedCache distributedCache, ILogger<ShowDetailsService> logger, ITMDBClient tmdbClient)
    {
        _distributedCache = distributedCache;
        _logger = logger;
        _tmdbClient = tmdbClient;
    }
    public MediaDetailsDto.DetailsDto? UpdatePathAndVoteDetailsDto(MediaDetailsDto.DetailsDto? detailsDto)
    {
        if (detailsDto == null)
        {
            return null;
        }

        detailsDto = detailsDto with
        {
            PosterPath = $"/tmdb/image{detailsDto.PosterPath}",
            BackdropPath = $"/tmdb/image{detailsDto.BackdropPath}",
            VoteAverage = Math.Round(detailsDto.VoteAverage, 1)
        };
        return detailsDto;
    }

    public Task<MediaDetailsDto.DetailsDto?> GetDetailsDtoCachedAsync(TvShow show, CancellationToken cancellationToken)
    {
        if (!show.TmdbId.HasValue)
        {
            return Task.FromResult<MediaDetailsDto.DetailsDto?>(null);
        }

        return DistributedCacheExtensions.GetSertAsync(_distributedCache, $"details-v1-{show.Id}", async () =>
        {
            MediaDetailsDto.DetailsDto? detailsDto = null;

            switch (show.Type)
            {
                case ShowType.Show:
                {
                    var showDetails = await _tmdbClient.GetShowDetailsByIdAsync(show.TmdbId.Value, cancellationToken);
                    if (showDetails == null)
                    {
                        LoggerExtensions.LogWarning(_logger, "Couldn't find show details for TMDB {id}", show.TmdbId);
                        break;
                    }

                    ;
                    int? year = DateTime.TryParse((string?)showDetails.FirstAirDate, out var releaseDate) ? releaseDate.Year : null;

                    detailsDto = new MediaDetailsDto.DetailsDto(showDetails.PosterPath,
                        showDetails.Overview,
                        showDetails.OriginalName,
                        (MediaDetailsDto.MediaType)show.Type,
                        showDetails.BackdropPath,
                        showDetails.VoteAverage,
                        Enumerable.Select<Genre, string>(showDetails.Genres, genre => genre.Name).ToArray(),
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
                        LoggerExtensions.LogWarning(_logger, "Couldn't find movie details for TMDB {id}", show.TmdbId);
                        break;
                    }

                    int? year = DateTime.TryParse((string?)movieDetails.ReleaseDate, out var releaseDate) ? releaseDate.Year : null;

                    detailsDto = new MediaDetailsDto.DetailsDto(movieDetails.PosterPath,
                        movieDetails.Overview,
                        movieDetails.OriginalTitle,
                        (MediaDetailsDto.MediaType)show.Type,
                        movieDetails.BackdropPath,
                        movieDetails.VoteAverage,
                        Enumerable.Select<Genre, string>(movieDetails.Genres, genre => genre.Name).ToArray(),
                        movieDetails.Tagline,
                        year,
                        movieDetails.Title);
                }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            detailsDto = UpdatePathAndVoteDetailsDto(detailsDto);
            return new DistributedCacheExtensions.CacheData<MediaDetailsDto.DetailsDto?>(detailsDto,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(14)
                });
        });
    }
}