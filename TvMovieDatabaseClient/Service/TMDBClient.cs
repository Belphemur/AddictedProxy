﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using TvMovieDatabaseClient.Bootstrap.EnvVar;
using TvMovieDatabaseClient.Model;
using TvMovieDatabaseClient.Model.Mapping;
using TvMovieDatabaseClient.Model.Movie;
using TvMovieDatabaseClient.Model.Movie.Search;
using TvMovieDatabaseClient.Model.Show;
using TvMovieDatabaseClient.Model.Show.Search;
using TvMovieDatabaseClient.Service.Model;

namespace TvMovieDatabaseClient.Service;

internal class TMDBClient : ITMDBClient
{
    private readonly TmdbConfig _tmdbConfig;
    private readonly HttpClient _httpClient;
    private readonly ILogger<TMDBClient> _logger;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        TypeInfoResolver = JsonTypeInfoResolver.Combine(JsonContext.Default, new DefaultJsonTypeInfoResolver())
    };

    private readonly Version clientVersion;

    public TMDBClient(TmdbConfig tmdbConfig, HttpClient httpClient, ILogger<TMDBClient> logger)
    {
        _tmdbConfig = tmdbConfig;
        _httpClient = httpClient;
        _logger = logger;
        clientVersion = Assembly.GetEntryAssembly()?.GetName().Version ?? new Version(1, 0, 0);
    }


    /// <summary>
    /// Get show details by Id
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<ShowDetails?> GetShowDetailsByIdAsync(int showId, CancellationToken token)
    {
        var request = PrepareRequest($"tv/{showId}", HttpMethod.Get);
        return await GetDataAsync<ShowDetails>(request, token);
    }

    /// <summary>
    /// Get show external ids by Id
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<ExternalIds?> GetShowExternalIdsAsync(int showId, CancellationToken token)
    {
        var request = PrepareRequest($"tv/{showId}/external_ids", HttpMethod.Get);
        return await GetDataAsync<ExternalIds>(request, token);
    }

    /// <summary>
    /// Get movie details by Id
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<MovieDetails?> GetMovieDetailsByIdAsync(int showId, CancellationToken token)
    {
        var request = PrepareRequest($"movie/{showId}", HttpMethod.Get);
        return await GetDataAsync<MovieDetails>(request, token);
    }

    /// <summary>
    /// Get movie external ids by Id
    /// </summary>
    /// <param name="movieId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<ExternalIds?> GetMovieExternalIdsAsync(int movieId, CancellationToken token)
    {
        var request = PrepareRequest($"movie/{movieId}/external_ids", HttpMethod.Get);
        return await GetDataAsync<ExternalIds>(request, token);
    }

    private async Task<T?> GetDataAsync<T>(HttpRequestMessage request, CancellationToken token) where T : class
    {
        var response = await _httpClient.SendAsync(request, token);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Couldn't get the show from TMDB: {request}", request.RequestUri);
            return null;
        }

        return await response.Content.ReadFromJsonAsync<T?>(cancellationToken: token, options: JsonSerializerOptions);
    }


    /// <summary>
    /// Search for tv shows
    /// </summary>
    /// <param name="query">query to send</param>
    /// <param name="token"></param>
    /// <returns></returns>
    public IAsyncEnumerable<ShowSearchResult> SearchTvAsync(string query, CancellationToken token)
    {
        const string searchType = "tv";
        return SearchAsync<ShowSearchResult>(searchType, query, token);
    }

    /// <summary>
    /// Get trending tv shows
    /// </summary>
    /// <param name="timeWindow"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public IAsyncEnumerable<ShowSearchResult> GetTrendingTvAsync(TimeWindowEnum timeWindow = TimeWindowEnum.week, CancellationToken token = default)
    {
        return PaginateAsync<ShowSearchResult>($"trending/tv/{timeWindow.ToString()}", new Dictionary<string, string>(), token);
    }

    /// <summary>
    /// Search for movie
    /// </summary>
    /// <param name="query">query to send</param>
    /// <param name="token"></param>
    /// <returns></returns>
    public IAsyncEnumerable<MovieSearchResult> SearchMovieAsync(string query, CancellationToken token)
    {
        const string searchType = "movie";
        return SearchAsync<MovieSearchResult>(searchType, query, token);
    }

    private async IAsyncEnumerable<T> SearchAsync<T>(string searchType, string query, CancellationToken token) => PaginateAsync<T>("search/" + searchType, new Dictionary<string, string> { { "query", query } }, token);

    private async IAsyncEnumerable<T> PaginateAsync<T>(string url, Dictionary<string, string> query, [EnumeratorCancellation] CancellationToken token)
    {
        var page = 1;
        const int maxPage = 3;
        HttpResponseMessage response;
        Pagination<T>? results;

        do
        {
            var queryParams = query.Union(new Dictionary<string, string>
                                   {
                                       { "language", "en-US" },
                                       { "page", page.ToString() },
                                   })
                                   .ToDictionary(x => x.Key, x => x.Value);

            var request = PrepareRequest(url, HttpMethod.Get, queryParams);
            response = await _httpClient.SendAsync(request, token);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Couldn't get a proper response from TMDB: {request}", request.RequestUri);
                yield break;
            }

            results = await response.Content.ReadFromJsonAsync<Pagination<T>>(cancellationToken: token, options: JsonSerializerOptions);
            if (results == null)
            {
                _logger.LogWarning("Couldn't parse the JSON from TMDB: {request}", request.RequestUri);
                yield break;
            }

            foreach (var result in results.Results)
            {
                yield return result;
            }

            page++;
        } while (response.IsSuccessStatusCode && page < results.TotalPages && page < maxPage);
    }

    private HttpRequestMessage PrepareRequest(string url, HttpMethod method, Dictionary<string, string>? queryParams = null)
    {
        queryParams ??= new Dictionary<string, string>();
        queryParams["api_key"] = _tmdbConfig.ApiKey;
        var urlWithQueryParams = QueryHelpers.AddQueryString(url, queryParams);
        return new HttpRequestMessage(method, urlWithQueryParams)
        {
            Headers = { { "User-Agent", $"TmdbClientNet/{clientVersion}" } }
        };
    }
}