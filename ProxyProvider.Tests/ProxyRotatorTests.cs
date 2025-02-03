using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ProxyProvider.GeoList.Bootstrap;
using ProxyProvider.GeoList.Service;
using ProxyProvider.Root.Bootstrap;
using ProxyProvider.Root.Service.Rotator;

namespace ProxyProvider.Tests;

[Ignore("Manual test")]
[TestFixture]
public class ProxyRotatorTests
{
    private ServiceCollection _services;

    [SetUp]
    public void Setup()
    {
        _services = [];
        var configuration = new ConfigurationBuilder().Build();
        new BootstrapGeoList().ConfigureServices(_services, configuration, Substitute.For<ILoggingBuilder>());
        new BootstrapProxyProvider().ConfigureServices(_services, configuration, Substitute.For<ILoggingBuilder>());
    }
    
    [Test]
    public async Task GetProxy()
    {
        // Arrange
        var serviceProvider = _services.BuildServiceProvider();
        var proxyRotator = serviceProvider.GetRequiredService<IProxyRotator>();
        await proxyRotator.UpdateProxiesAsync(CancellationToken.None);
        
        // Act
        var proxy = await proxyRotator.GetNextProxyAsync(CancellationToken.None);
        
        // Assert
        Assert.That(proxy, Is.Not.Null);
    }
}