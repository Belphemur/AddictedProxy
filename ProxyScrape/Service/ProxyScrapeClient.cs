using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using AntiCaptcha.Model.Error;
using AntiCaptcha.Model.Task.Turnstile;
using AntiCaptcha.Service;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using ProxyScrape.Json;
using ProxyScrape.Model;
using ProxyScrape.Utils;

namespace ProxyScrape.Service;

public class ProxyScrapeClient : IProxyScrapeClient
{
    private const string AuthResponseCachingKey = "proxy-scrape-login-data/v2";
    private readonly IOptions<ProxyScrapeConfig> _config;
    private readonly HttpClient _client;
    private readonly IAntiCaptchaClient _antiCaptchaClient;
    private readonly IDistributedCache _cache;


    public ProxyScrapeClient(IOptions<ProxyScrapeConfig> config, HttpClient client, IAntiCaptchaClient antiCaptchaClient, IDistributedCache cache)
    {
        _config = config;
        _client = client;
        _antiCaptchaClient = antiCaptchaClient;
        _cache = cache;
    }

    private async Task<TurnstileSolution?> GetCfTokenAsync(CancellationToken token)
    {
        var response = await _antiCaptchaClient.SolveTurnstileProxylessAsync(new TurnstileProxylessTask
        {
            WebsiteKey = "0x4AAAAAAAFWUVCKyusT9T8r",
            WebsiteUrl = "https://dashboard.proxyscrape.com/v2/login",
        }, token);
        return response?.Solution;
    }

    private static string? ExtractCookieValue(string setCookieHeader)
    {
        if (string.IsNullOrEmpty(setCookieHeader))
            return null;

        // Get the value part before any attributes (before first semicolon)
        var mainValue = setCookieHeader.Split(';').FirstOrDefault() ?? string.Empty;

        // Get the value after the equals sign
        var parts = mainValue.Split('=', 2);
        return parts.Length > 1 ? parts[1].Trim() : null;
    }

    private async Task<AuthResponse?> GetLoginDataAsync(CancellationToken token)
    {
        var authResponse = await _cache.GetAsync<AuthResponse>(AuthResponseCachingKey, token);
        if (authResponse != null)
        {
            return authResponse;
        }

        var cfToken = await GetCfTokenAsync(token);
        if (cfToken is null)
        {
            return null;
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "v2/v4/account/auth/login");
        var guid = Guid.NewGuid().ToString("N");
        var boundary = $"geckoformboundary{guid}";
        request.Content = new StringContent($"""
                                             ------{boundary}
                                             Content-Disposition: form-data; name="email"

                                             {_config.Value.User.Username}
                                             ------{boundary}
                                             Content-Disposition: form-data; name="password"

                                             {_config.Value.User.Password}
                                             ------{boundary}
                                             Content-Disposition: form-data; name="сf_trustile_token"

                                             {cfToken.Value.Token}
                                             ------{boundary}--

                                             """,
            MediaTypeHeaderValue.Parse($"multipart/form-data; boundary=----{boundary}"));
        request.Headers.UserAgent.ParseAdd(cfToken.Value.UserAgent);
        request.Headers.Referrer = new Uri("https://dashboard.proxyscrape.com/v2/login");
        request.Headers.Add("Origin", "https://dashboard.proxyscrape.com");
        var response = await _client.SendAsync(request, token);
        response.EnsureSuccessStatusCode();

        authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonContext.JsonSerializerOptions, token);
        authResponse!.UserAgent = cfToken.Value.UserAgent;

        await _cache.SetAsync(AuthResponseCachingKey, authResponse, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(authResponse.ExpiresIn)
        }, token);
        return authResponse;
    }

    /// <summary>
    /// Get proxy statistics
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException">If the request wasn't successfull</exception>
    public async Task<ProxyStatistics?> GetProxyStatisticsAsync(CancellationToken token)
    {
        var loginExtraData = await GetLoginDataAsync(token);
        if (loginExtraData is null)
        {
            return null;
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"/v2/v4/account/{_config.Value.AccountId}/residential/subuser/{_config.Value.SubUserId}/statistic");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginExtraData.AccessToken);
        request.Headers.UserAgent.ParseAdd(loginExtraData.UserAgent);
        request.Headers.Add("Referer", $"https://dashboard.proxyscrape.com/v2/services/residential/overview/{_config.Value.AccountId}");
        var response = await _client.SendAsync(request, token);
        if (response.StatusCode == HttpStatusCode.Redirect)
        {
            await _cache.RemoveAsync(AuthResponseCachingKey, token);
            return await GetProxyStatisticsAsync(token).ConfigureAwait(false);
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProxyStatistics>(JsonContext.JsonSerializerOptions, token);
    }
}