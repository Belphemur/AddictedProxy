﻿using AddictedProxy.Caching.Extensions;
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
using AddictedProxy.Services.Details;
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

    public MediaController(IShowRefresher showRefresher, ITMDBClient tmdbClient, ITvShowRepository tvShowRepository, IDistributedCache distributedCache, IMediaDetailsService mediaDetailsService)
    {
        _showRefresher = showRefresher;
        _tmdbClient = tmdbClient;
        _tvShowRepository = tvShowRepository;
        _distributedCache = distributedCache;
        _mediaDetailsService = mediaDetailsService;
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
}