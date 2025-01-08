using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProxyProvider.GeoList.Bootstrap;
using ProxyProvider.Root.Bootstrap;
using ProxyProvider.Root.Service.HttpHandler;
using ProxyProvider.Root.Service.Rotator;

namespace ProxyProvider.Tests;

[Ignore("Manual test")]
[TestFixture]
public class HttpHandlerTests
{
    private ServiceCollection _services;

    [SetUp]
    public void Setup()
    {
        _services = [];
        var configuration = new ConfigurationBuilder().Build();
        new BootstrapGeoList().ConfigureServices(_services, configuration);
        new BootstrapProxyProvider().ConfigureServices(_services, configuration);
    }
    
   
    
    [Test]
    public async Task HttpHandler_UsesProxy()
    {
        // Arrange
        var serviceProvider = _services.BuildServiceProvider();
        var proxyRotator = serviceProvider.GetRequiredService<IProxyRotator>();
        await proxyRotator.UpdateProxiesAsync(CancellationToken.None);

        var handler = new ProxyRotatorHttpHandler(proxyRotator);

        using var httpClient = new HttpClient(handler);

        // Act
        var response = await httpClient.GetAsync("http://ifconfig.co");

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}