using AntiCaptcha.Bootstrap;
using AntiCaptcha.Model.Task.Turnstile;
using AntiCaptcha.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace AntiCaptcha.Tests;

[Ignore("Only run manually to not use quota")]
public class Tests
{
    private ServiceCollection _services;

    [SetUp]
    public void Setup()
    {
        _services = new ServiceCollection();
        var config = new ConfigurationBuilder().Add(new MemoryConfigurationSource
        {
            InitialData = new Dictionary<string, string>
            {
                {"AntiCaptcha:ClientKey", Environment.GetEnvironmentVariable("ANTI_CAPTCHA_CLIENT_KEY")!}
            }!
        }).Build();
        new BootstrapAntiCaptcha().ConfigureServices(_services, config);
    }

    [Test]
    public async Task SolveProxyScrape()
    {
        var provider = _services.BuildServiceProvider();
        var client = provider.GetRequiredService<IAntiCaptchaClient>();
        var response = await client.SolveTurnstileProxylessAsync(new TurnstileProxylessTask
        {
            WebsiteUrl = "https://dashboard.proxyscrape.com/",
            WebsiteKey = "0x4AAAAAAAFWUVCKyusT9T8r"
        });
        Assert.Multiple(() =>
        {
            Assert.That(response.HasValue, Is.True);
            Assert.That(response!.Value.Solution, Is.Not.Default);
        });
    }
}
