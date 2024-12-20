using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProxyProvider.GeoList.Bootstrap;
using ProxyProvider.GeoList.Service;
using ProxyProvider.Root.Bootstrap;
using ProxyProvider.Root.Service.Rotator;

namespace ProxyProvider.Tests;

[TestFixture]
public class Test
{
    private ServiceCollection _services;

    [SetUp]
    public void Setup()
    {
        _services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        new BootstrapGeoList().ConfigureServices(_services, configuration);
        new BootstrapProxyProvider().ConfigureServices(_services, configuration);
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