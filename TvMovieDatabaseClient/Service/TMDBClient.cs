using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using TvMovieDatabaseClient.Bootstrap.EnvVar;
using TvMovieDatabaseClient.Model;
using TvMovieDatabaseClient.Model.Search;
using TvMovieDatabaseClient.Model.Show;

namespace TvMovieDatabaseClient.Service;

internal class TMDBClient : ITMDBClient
{
    private readonly TmdbConfig _tmdbConfig;
    private readonly HttpClient _httpClient;
    private readonly ILogger<TMDBClient> _logger;

    public TMDBClient(TmdbConfig tmdbConfig, HttpClient httpClient, ILogger<TMDBClient> logger)
    {
        _tmdbConfig = tmdbConfig;
        _httpClient = httpClient;
        _logger = logger;
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
        var response = await _httpClient.SendAsync(request, token);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Couldn't get the show from TMDB: {request}", request.RequestUri);
            return null;
        }

        return await response.Content.ReadFromJsonAsync(JsonContext.Default.ShowDetails, cancellationToken: token);
    }

    /// <summary>
    /// Search for tv shows
    /// </summary>
    /// <param name="query">query to send</param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<ShowSearchResult> SearchTvAsync(string query, CancellationToken token)
    {
        var page = 1;
        HttpResponseMessage response;
        Pagination<ShowSearchResult>? results;
        do
        {
            var request = PrepareRequest("search/tv", HttpMethod.Get, new Dictionary<string, string>
            {
                { "language", "en-US" },
                { "page", page.ToString() },
                { "query", query }
            });
            response = await _httpClient.SendAsync(request, token);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Couldn't get a proper response from TMDB: {request}", request.RequestUri);
                yield break;
            }

            results = await response.Content.ReadFromJsonAsync(JsonContext.Default.PaginationShowSearchResult, cancellationToken: token);
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
        } while (response.IsSuccessStatusCode && results.TotalPages <= page && results.TotalPages != 0);
    }

    private HttpRequestMessage PrepareRequest(string url, HttpMethod method, Dictionary<string, string>? queryParams = null)
    {
        queryParams ??= new Dictionary<string, string>();
        queryParams["api_key"] = _tmdbConfig.ApiKey;
        var urlWithQueryParams = QueryHelpers.AddQueryString(url, queryParams);
        return new HttpRequestMessage(method, urlWithQueryParams)
        {
            Headers = { { "User-Agent", $"TmdbClientNet/{Assembly.GetExecutingAssembly().GetName().Version}" } }
        };
    }
}