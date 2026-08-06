#region

using System.Net;
using FluentAssertions;
using InversionOfControl.Service.Resilience;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace AddictedProxy.Resilience.Tests;

[TestFixture]
public class ResilienceDefaultsTests
{
    [Test]
    public void AddSharedResilienceHandler_RegistersHttpClientWithResilienceHandler()
    {
        var services = new ServiceCollection();
        services.AddHttpClient("test")
                .AddSharedResilienceHandler("test-pipeline");

        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();

        var client = factory.CreateClient("test");

        client.Should().NotBeNull();
    }

    [TestCase(HttpStatusCode.InternalServerError)]
    [TestCase(HttpStatusCode.BadGateway)]
    [TestCase(HttpStatusCode.ServiceUnavailable)]
    [TestCase(HttpStatusCode.GatewayTimeout)]
    [TestCase(HttpStatusCode.Unauthorized)]
    [TestCase(HttpStatusCode.PaymentRequired)]
    [TestCase(HttpStatusCode.Forbidden)]
    public async Task TrippedStatusCodes_AreRetried(HttpStatusCode statusCode)
    {
        var services = new ServiceCollection();
        var handler = new CountingHandler(statusCode);
        services.AddHttpClient("test", client => client.BaseAddress = new Uri("https://example.com"))
                .ConfigurePrimaryHttpMessageHandler(() => handler)
                .AddSharedResilienceHandler("test-pipeline", TimeSpan.FromSeconds(1));

        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();
        var client = factory.CreateClient("test");

        var response = await client.GetAsync("/");

        response.StatusCode.Should().Be(statusCode);
        handler.Attempts.Should().Be(4);
    }

    [TestCase(HttpStatusCode.OK)]
    [TestCase(HttpStatusCode.NotFound)]
    [TestCase(HttpStatusCode.BadRequest)]
    [TestCase(HttpStatusCode.TooManyRequests)]
    public async Task NonTrippedStatusCodes_AreNotRetried(HttpStatusCode statusCode)
    {
        var services = new ServiceCollection();
        var handler = new CountingHandler(statusCode);
        services.AddHttpClient("test", client => client.BaseAddress = new Uri("https://example.com"))
                .ConfigurePrimaryHttpMessageHandler(() => handler)
                .AddSharedResilienceHandler("test-pipeline", TimeSpan.FromSeconds(1));

        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();
        var client = factory.CreateClient("test");

        try
        {
            await client.GetAsync("/");
        }
        catch
        {
            // Some non-success statuses may still throw when EnsureSuccessStatusCode is used elsewhere,
            // but the resilience handler itself should not retry.
        }

        handler.Attempts.Should().Be(1);
    }

    [Test]
    public async Task PostRequests_AreNotRetried()
    {
        var services = new ServiceCollection();
        var handler = new CountingHandler(HttpStatusCode.InternalServerError);
        services.AddHttpClient("test", client => client.BaseAddress = new Uri("https://example.com"))
                .ConfigurePrimaryHttpMessageHandler(() => handler)
                .AddSharedResilienceHandler("test-pipeline", TimeSpan.FromSeconds(1));

        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();
        var client = factory.CreateClient("test");

        try
        {
            await client.PostAsync("/", new StringContent("payload"));
        }
        catch
        {
            // Expected when the request fails.
        }

        handler.Attempts.Should().Be(1);
    }

    [Test]
    public async Task PostRequests_WithIdempotencyKey_AreRetried()
    {
        var services = new ServiceCollection();
        var handler = new CountingHandler(HttpStatusCode.InternalServerError);
        services.AddHttpClient("test", client => client.BaseAddress = new Uri("https://example.com"))
                .ConfigurePrimaryHttpMessageHandler(() => handler)
                .AddSharedResilienceHandler("test-pipeline", TimeSpan.FromSeconds(1));

        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();
        var client = factory.CreateClient("test");
        client.DefaultRequestHeaders.Add("Idempotency-Key", Guid.NewGuid().ToString("N"));

        var response = await client.PostAsync("/", new StringContent("payload"));

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        handler.Attempts.Should().Be(4);
    }

    [Test]
    public async Task HttpRequestException_IsRetried()
    {
        var services = new ServiceCollection();
        var handler = new ThrowingHandler<HttpRequestException>();
        services.AddHttpClient("test", client => client.BaseAddress = new Uri("https://example.com"))
                .ConfigurePrimaryHttpMessageHandler(() => handler)
                .AddSharedResilienceHandler("test-pipeline", TimeSpan.FromSeconds(1));

        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();
        var client = factory.CreateClient("test");

        await client.Invoking(c => c.GetAsync("/")).Should().ThrowAsync<HttpRequestException>();

        handler.Attempts.Should().Be(4);
    }

    private class CountingHandler : DelegatingHandler
    {
        private readonly HttpStatusCode _statusCode;

        public CountingHandler(HttpStatusCode statusCode)
        {
            _statusCode = statusCode;
        }

        public int Attempts { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Attempts++;
            return Task.FromResult(new HttpResponseMessage(_statusCode));
        }
    }

    private class ThrowingHandler<TException> : DelegatingHandler where TException : Exception, new()
    {
        public int Attempts { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Attempts++;
            return Task.FromException<HttpResponseMessage>(new TException());
        }
    }
}
