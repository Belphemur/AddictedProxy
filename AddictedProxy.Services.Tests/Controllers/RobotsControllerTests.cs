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

    [Test]
    public void Index_Always_DeclaresExplicitGroupForEachAiCrawler()
    {
        var controller = CreateController("https://www.subvault.tv");

        var result = controller.Index();

        foreach (var agent in AddictedProxy.Controllers.Sitemap.RobotsController.AiCrawlerUserAgents)
        {
            result.Content.Should().Contain($"User-Agent: {agent}");
        }
    }

    [Test]
    public void Index_AiCrawlerAgents_AreSingleStaticList()
    {
        AddictedProxy.Controllers.Sitemap.RobotsController.AiCrawlerUserAgents.Should().BeEquivalentTo(
            "GPTBot",
            "OAI-SearchBot",
            "ChatGPT-User",
            "PerplexityBot",
            "ClaudeBot",
            "Claude-Web",
            "Google-Extended",
            "CCBot",
            "Bytespider");
    }

    [Test]
    public void Index_EveryAgentGroup_DisallowsSubtitleDownloadPath()
    {
        var controller = CreateController("https://www.subvault.tv");

        var result = controller.Index();

        var content = result.Content!;
        content.Should().Contain("Disallow: /subtitles/download/");
        // One wildcard group + one AI-crawler group = two Disallow blocks.
        content.Split("Disallow: /subtitles/download/").Length.Should().Be(3, "wildcard and AI-crawler groups each disallow the download path");
    }

    [Test]
    public void Index_Always_PointsAgentsAtLlmsTxt()
    {
        var controller = CreateController("https://www.subvault.tv");

        var result = controller.Index();

        result.Content.Should().Contain("/llms.txt");
    }

    [Test]
    public void Index_Always_KeepsSitemapLast()
    {
        var controller = CreateController("https://www.subvault.tv");

        var result = controller.Index();

        var content = result.Content!;
        content.Should().Contain("Sitemap: https://www.subvault.tv/sitemap.xml");
        content.IndexOf("Sitemap:").Should().BeGreaterThan(content.IndexOf("Disallow: /subtitles/download/"));
    }
}
