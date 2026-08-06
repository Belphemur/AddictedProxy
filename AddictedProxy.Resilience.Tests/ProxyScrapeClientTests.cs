#region

using System.Net;
using AntiCaptcha.Service;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NSubstitute;
using ProxyScrape.Model;
using ProxyScrape.Service;
using ProxyScrape.Utils;

#endregion

namespace AddictedProxy.Resilience.Tests;

[TestFixture]
public class ProxyScrapeClientTests
{
    private IProxyScrapeClient _sut = null!;
    private IOptions<ProxyScrapeConfig> _config = null!;
    private HttpClient _httpClient = null!;
    private IAntiCaptchaClient _antiCaptchaClient = null!;
    private MemoryDistributedCache _cache = null!;
    private TestMessageHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _handler = new TestMessageHandler();
        _config = Options.Create(new ProxyScrapeConfig
        {
            AccountId = "account",
            SubUserId = "subuser",
            User = new ProxyScrapeConfig.Creds { Username = "user", Password = "pass" }
        });
        _httpClient = new HttpClient(_handler)
        {
            BaseAddress = new Uri("https://dashboard.proxyscrape.com/")
        };
        _antiCaptchaClient = Substitute.For<IAntiCaptchaClient>();
        _cache = new MemoryDistributedCache(new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));
        _sut = new ProxyScrapeClient(_config, _httpClient, _antiCaptchaClient, _cache);
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient.Dispose();
        _handler.Dispose();
    }

    [Test]
    public async Task GetProxyOverviewAsync_Unauthorized_InvalidatesCache()
    {
        _handler.ResponseFactory = _ => new HttpResponseMessage(HttpStatusCode.Unauthorized);
        await SetAuthResponseAsync(_cache, "proxy-scrape-login-data/v2", new AuthResponse
        {
            AccessToken = "token",
            ExpiresIn = 3600,
            TokenType = "Bearer",
            UserAgent = "ua"
        }, new DistributedCacheEntryOptions());

        var result = await _sut.GetProxyOverviewAsync(CancellationToken.None);

        result.Should().BeNull();
        (await _cache.GetAsync("proxy-scrape-login-data/v2")).Should().BeNull();
    }

    [Test]
    public async Task GetProxyOverviewAsync_Forbidden_InvalidatesCache()
    {
        _handler.ResponseFactory = _ => new HttpResponseMessage(HttpStatusCode.Forbidden);
        await SetAuthResponseAsync(_cache, "proxy-scrape-login-data/v2", new AuthResponse
        {
            AccessToken = "token",
            ExpiresIn = 3600,
            TokenType = "Bearer",
            UserAgent = "ua"
        }, new DistributedCacheEntryOptions());

        var result = await _sut.GetProxyOverviewAsync(CancellationToken.None);

        result.Should().BeNull();
        (await _cache.GetAsync("proxy-scrape-login-data/v2")).Should().BeNull();
    }

    private static async Task SetAuthResponseAsync(IDistributedCache cache, string key, AuthResponse response, DistributedCacheEntryOptions options)
    {
        var bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(response);
        await cache.SetAsync(key, bytes, options);
    }

    private class TestMessageHandler : DelegatingHandler
    {
        public Func<HttpRequestMessage, HttpResponseMessage>? ResponseFactory { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = ResponseFactory?.Invoke(request) ?? new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                                            {
                                                "status": "ok",
                                                "data": {
                                                    "account_type": "residential",
                                                    "bandwidth": 0,
                                                    "campaign_id": null,
                                                    "discount_applied": 0,
                                                    "duration": "monthly",
                                                    "email": "test@example.com",
                                                    "expiry_date": 0,
                                                    "external_parent_id": 0,
                                                    "external_sub_user_id": 0,
                                                    "id": "id",
                                                    "time_created": 0,
                                                    "users": [],
                                                    "plans": []
                                                },
                                                "account_type": "residential",
                                                "riptide_api_response": []
                                            }
                                            """)
            };
            return Task.FromResult(response);
        }
    }
}
