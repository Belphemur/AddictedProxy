#region

using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Options;
using ProxyScrape.Model;
using ProxyScrape.Service;

#endregion

namespace AddictedProxy.Resilience.Tests;

[TestFixture]
public class ProxyScrapeClientTests
{
    private const string StatisticJson =
        """{"success":true,"remaining_data":9000000000000000000,"remaining_traffic":10000000000,"used_data":15000000000}""";

    private const string OverviewJson =
        """{"status":"valid","data":{"account_type":"residential","bandwidth":50,"campaign_id":null,"discount_applied":0,"duration":"30d","email":"test@example.com","expiry_date":1893456000,"external_parent_id":10001,"external_sub_user_id":10002,"id":"00000000-0000-4000-8000-000000000000","time_created":1700000000,"users":[{"pp_api_login":"test_pp_login","pp_api_password":"test_pp_password","proxy_ip":"proxy.example.com","proxy_port":"6060"}],"plans":[{"id":20001,"max_bytes":50000000000,"bytes_used":8000000000,"max_threads":10,"max_throughput":1000,"start_date":"2025-01-01 00:00:00","end_date":"2025-02-01 00:00:00","status":"expired","duration":12600,"bandwidth_gb":50},{"id":20002,"max_bytes":20000000000,"bytes_used":9000000000,"max_threads":10,"max_throughput":1000,"start_date":"2026-01-01 00:00:00","end_date":"2026-02-01 00:00:00","status":"active","duration":26280000,"bandwidth_gb":20}]},"account_type":"residential","riptide_api_response":[{"id":20001,"parent_id":10001,"max_bytes":50000000000,"used_bytes":8000000000,"expiry_time":1893456000,"duration":12600,"status":"expired","created_at":"2025-01-01 00:00:00"},{"id":20002,"parent_id":10001,"max_bytes":20000000000,"used_bytes":9000000000,"expiry_time":1893456000,"duration":26280000,"status":"active","created_at":"2026-01-01 00:00:00"}]}""";

    private IProxyScrapeClient _sut = null!;
    private HttpClient _httpClient = null!;
    private TestMessageHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _handler = new TestMessageHandler();
        var config = Options.Create(new ProxyScrapeConfig
        {
            AccountId = "account",
            SubUserId = "subuser",
            ApiToken = "test-token"
        });
        _httpClient = new HttpClient(_handler)
        {
            BaseAddress = new Uri("https://api.proxyscrape.com/")
        };
        _sut = new ProxyScrapeClient(config, _httpClient);
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient.Dispose();
        _handler.Dispose();
    }

    [Test]
    public async Task GetProxyStatisticsAsync_SendsApiTokenAndDeserializesResponse()
    {
        _handler.ResponseFactory = _ => JsonResponse(StatisticJson);

        var result = await _sut.GetProxyStatisticsAsync(CancellationToken.None);

        _handler.LastRequestPath.Should().Be("/v4/account/account/residential/subuser/subuser/statistic");
        _handler.LastApiToken.Should().Be("test-token");
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.RemainingData.Should().Be(9000000000000000000L);
        result.RemainingTraffic.Should().Be(10000000000L);
        result.UsedData.Should().Be(15000000000L);
    }

    [Test]
    public async Task GetProxyOverviewAsync_SendsApiTokenAndDeserializesResponse()
    {
        _handler.ResponseFactory = _ => JsonResponse(OverviewJson);

        var result = await _sut.GetProxyOverviewAsync(CancellationToken.None);

        _handler.LastRequestPath.Should().Be("/v4/account/account/residential/overview");
        _handler.LastApiToken.Should().Be("test-token");
        result.Should().NotBeNull();
        result!.Status.Should().Be("valid");
        result.Data.Plans.Should().HaveCount(2);
        result.Data.Users[0].PpApiLogin.Should().Be("test_pp_login");
    }

    [Test]
    public async Task GetProxyStatisticsAsync_Unauthorized_ThrowsHttpRequestException()
    {
        _handler.ResponseFactory = _ => new HttpResponseMessage(HttpStatusCode.Unauthorized);

        var act = () => _sut.GetProxyStatisticsAsync(CancellationToken.None);

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    private static HttpResponseMessage JsonResponse(string json) => new(HttpStatusCode.OK)
    {
        Content = new StringContent(json, Encoding.UTF8, "application/json")
    };

    private class TestMessageHandler : DelegatingHandler
    {
        public Func<HttpRequestMessage, HttpResponseMessage>? ResponseFactory { get; set; }
        public string? LastRequestPath { get; private set; }
        public string? LastApiToken { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequestPath = request.RequestUri?.AbsolutePath;
            LastApiToken = request.Headers.TryGetValues("api-token", out var values) ? values.Single() : null;
            var response = ResponseFactory?.Invoke(request) ?? new HttpResponseMessage(HttpStatusCode.OK);
            return Task.FromResult(response);
        }
    }
}
