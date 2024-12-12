using System.Net.Http.Json;
using Microsoft.Extensions.Options;
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
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException">If the request wasn't successfull</exception>
    public async Task<ProxyStatistics?> GetProxyStatisticsAsync(CancellationToken token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"account/{_config.Value.AccountId}/residential/subuser/{_config.Value.SubUserId}/statistic");
        request.Headers.Add("Cookie", $"PHPSESSID={_config.Value.PhpSessionId}");
        request.Headers.Add("Referer", $"https://dashboard.proxyscrape.com/v2/services/residential/overview/{_config.Value.AccountId}");
        var response = await _client.SendAsync(request, token);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProxyStatistics>(cancellationToken: token);
    }
    
}