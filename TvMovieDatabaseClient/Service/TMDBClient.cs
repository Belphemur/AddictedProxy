using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using TvMovieDatabaseClient.Bootstrap.EnvVar;
using TvMovieDatabaseClient.Model;
using TvMovieDatabaseClient.Model.Search;

namespace TvMovieDatabaseClient.Service;

internal class TMDBClient
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

    public async IAsyncEnumerator<ShowSearchResult> SearchTvAsync(string query, CancellationToken token)
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
        } while (response.IsSuccessStatusCode && results.TotalPages <= page);
    }

    private HttpRequestMessage PrepareRequest(string url, HttpMethod method, Dictionary<string, string> queryParams)
    {
        queryParams["apiKey"] = _tmdbConfig.ApiKey;
        return new HttpRequestMessage
        {
            RequestUri = new Uri(QueryHelpers.AddQueryString(url, queryParams)),
            Method = method
        };
    }
}