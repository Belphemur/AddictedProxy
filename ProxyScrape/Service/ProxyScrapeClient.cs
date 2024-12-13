using System.Net;
using System.Net.Http.Json;
using AntiCaptcha.Model.Error;
using AntiCaptcha.Model.Task.Turnstile;
using AntiCaptcha.Service;
using Microsoft.Extensions.Options;
using ProxyScrape.Model;

namespace ProxyScrape.Service;

public class ProxyScrapeClient : IProxyScrapeClient
{
    private readonly IOptions<ProxyScrapeConfig> _config;
    private readonly HttpClient _client;
    private readonly IAntiCaptchaClient _antiCaptchaClient;
    private LoginExtraData? _loginExtraData = null;

    public ProxyScrapeClient(IOptions<ProxyScrapeConfig> config, HttpClient client, IAntiCaptchaClient antiCaptchaClient)
    {
        _config = config;
        _client = client;
        _antiCaptchaClient = antiCaptchaClient;
    }

    private async Task<TurnstileSolution?> GetCfTokenAsync(CancellationToken token)
    {
        var response = await _antiCaptchaClient.SolveTurnstileProxylessAsync(new TurnstileProxylessTask
        {
            WebsiteKey = "0x4AAAAAAAFWUVCKyusT9T8r",
            WebsiteUrl = "https://dashboard.proxyscrape.com/"
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

    private async Task<LoginExtraData?> GetLoginDataAsync(CancellationToken token)
    {
        var cfToken = await GetCfTokenAsync(token);
        if (cfToken is null)
        {
            return null;
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "account-functions/dologin.php");
        request.Content = new MultipartFormDataContent
        {
            { new StringContent(_config.Value.User.Username), "email" },
            { new StringContent(_config.Value.User.Password), "password" },
            { new StringContent(cfToken.Value.Token), "cf-turnstile-response" },
        };
        request.Headers.UserAgent.ParseAdd(cfToken.Value.UserAgent);
        var response = await _client.SendAsync(request, token);
        var phpSessionId = ExtractCookieValue(response.Headers.GetValues("Set-Cookie").First(cookie => cookie.StartsWith("PHPSESSID=")));
        if (phpSessionId == null)
            return null;
        return new LoginExtraData(phpSessionId, cfToken.Value);
    }

    /// <summary>
    /// Get proxy statistics
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException">If the request wasn't successfull</exception>
    public async Task<ProxyStatistics?> GetProxyStatisticsAsync(CancellationToken token)
    {
        _loginExtraData ??= await GetLoginDataAsync(token);

        var request = new HttpRequestMessage(HttpMethod.Get, $"/v2/v4/account/{_config.Value.AccountId}/residential/subuser/{_config.Value.SubUserId}/statistic");
        request.Headers.Add("Cookie", $"PHPSESSID={_loginExtraData!.Value.PhpSessionId}");
        request.Headers.UserAgent.ParseAdd(_loginExtraData.Value.CaptchaSolution.UserAgent);
        request.Headers.Add("Referer", $"https://dashboard.proxyscrape.com/v2/services/residential/overview/{_config.Value.AccountId}");
        var response = await _client.SendAsync(request, token);
        if (response.StatusCode == HttpStatusCode.Redirect)
        {
            _loginExtraData = null;
            return await GetProxyStatisticsAsync(token).ConfigureAwait(false);
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProxyStatistics>(cancellationToken: token);
    }
}