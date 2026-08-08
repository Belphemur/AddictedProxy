using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using ProxyScrape.Json;
using ProxyScrape.Model;

namespace ProxyScrape.Service;

public class ProxyScrapeClient : IProxyScrapeClient
{
    private readonly IOptions<ProxyScrapeConfig> _config;
    private readonly HttpClient _client;

    public ProxyScrapeClient(IOptions<ProxyScrapeConfig> config, HttpClient client)
    {
        _config = config;
        _client = client;
    }

    /// <summary>
    /// Get proxy statistics
    /// </summary>
    public async Task<ProxyStatistics?> GetProxyStatisticsAsync(CancellationToken token)
    {
        return await SendAsync<ProxyStatistics>($"v4/account/{_config.Value.AccountId}/residential/subuser/{_config.Value.SubUserId}/statistic", token);
    }

    /// <summary>
    /// Get proxy overview
    /// </summary>
    public async Task<ProxyOverview?> GetProxyOverviewAsync(CancellationToken token)
    {
        return await SendAsync<ProxyOverview>($"v4/account/{_config.Value.AccountId}/residential/overview", token);
    }

    private async Task<TResult?> SendAsync<TResult>(string path, CancellationToken token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        request.Headers.Add("api-token", _config.Value.ApiToken);
        using var response = await _client.SendAsync(request, token);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResult>(JsonContext.JsonSerializerOptions, token);
    }
}
