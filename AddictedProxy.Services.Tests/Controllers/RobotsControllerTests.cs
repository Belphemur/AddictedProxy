using FluentAssertions;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace AddictedProxy.Services.Tests.Controllers;

[TestFixture]
public class RobotsControllerTests
{
    private static AddictedProxy.Controllers.Sitemap.RobotsController CreateController(string baseUrl)
    {
        return new AddictedProxy.Controllers.Sitemap.RobotsController(
            Options.Create(new AddictedProxy.Services.Sitemap.SitemapConfig { BaseUrl = baseUrl }));
    }

    [Test]
    public void Index_BaseUrlWithTrailingSlash_StripsSlashBeforeSitemapPath()
    {
        var controller = CreateController("https://www.subvault.tv/");

        var result = controller.Index();

        result.Content.Should().Contain("Sitemap: https://www.subvault.tv/sitemap.xml");
    }

    [Test]
    public void Index_BaseUrlWithoutTrailingSlash_AppendsSitemapPath()
    {
        var controller = CreateController("https://www.subvault.tv");

        var result = controller.Index();

        result.Content.Should().Contain("Sitemap: https://www.subvault.tv/sitemap.xml");
    }

    [Test]
    public void Index_Always_DeclaresCrawlRulesBeforeSitemap()
    {
        var controller = CreateController("https://www.subvault.tv");

        var result = controller.Index();

        var content = result.Content;
        content.Should().StartWith("User-Agent: *\n");
        content.Should().Contain("Allow: /");
        content.IndexOf("Allow: /").Should().BeLessThan(content.IndexOf("Sitemap:"));
    }
}