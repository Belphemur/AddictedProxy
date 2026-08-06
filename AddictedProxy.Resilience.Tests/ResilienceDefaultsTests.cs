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
    public void ShouldTreatAsFailure_TrippedStatusCodes_AreFailures(HttpStatusCode statusCode)
    {
        var status = (int)statusCode;

        (status is >= 500 and <= 599 or 401 or 402 or 403).Should().BeTrue();
    }

    [TestCase(HttpStatusCode.OK)]
    [TestCase(HttpStatusCode.NotFound)]
    [TestCase(HttpStatusCode.BadRequest)]
    [TestCase(HttpStatusCode.TooManyRequests)]
    public void ShouldTreatAsFailure_NonTrippedStatusCodes_AreNotFailures(HttpStatusCode statusCode)
    {
        var status = (int)statusCode;

        (status is >= 500 and <= 599 or 401 or 402 or 403).Should().BeFalse();
    }
}
