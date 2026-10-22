using System.Linq;
using AddictedProxy.Controllers.Rest;
using AddictedProxy.Controllers.Sitemap;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Sitemap;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSubstitute;
using SimpleMvcSitemap;
using NUnit.Framework;

namespace AddictedProxy.Services.Tests.Controllers;

[TestFixture]
public class SitemapControllerTests
{
    private ITvShowRepository _tvShowRepository = null!;
    private SitemapController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _tvShowRepository = Substitute.For<ITvShowRepository>();
        _controller = new SitemapController(
            Substitute.For<ISitemapProvider>(),
            Substitute.For<IDynamicSitemapIndexProvider>(),
            _tvShowRepository,
            Options.Create(new SitemapConfig { BaseUrl = "https://www.subvault.tv" }));
    }

    private static TvShow CreateShow(string name, Guid uniqueId)
    {
        return new TvShow { Id = 1, Name = name, UniqueId = uniqueId };
    }

    [Test]
    public void LlmCatalog_ReturnsTextPlainWithShows()
    {
        _tvShowRepository.GetAllHavingSubtitlesAsync().Returns(new[]
        {
            CreateShow("Breaking Bad", Guid.Parse("1f8a4b4f-4f4b-4f4b-4f4b-4f4b4f4b4f4b")),
        }.AsQueryable());

        var result = _controller.LlmCatalog();

        result.Content.Should().NotBeNullOrWhiteSpace();
        result.Content.Should().Contain("/shows/");
        result.Content.Should().StartWith("# ");
        result.Content.Should().Contain("## Links");
        result.Content.Should().Contain("- [Breaking Bad](");
    }

    [Test]
    public void LlmCatalog_NeverContainsDownloadUris()
    {
        _tvShowRepository.GetAllHavingSubtitlesAsync().Returns(new[]
        {
            CreateShow("Breaking Bad", Guid.NewGuid()),
        }.AsQueryable());

        var result = _controller.LlmCatalog();

        result.Content.Should().NotContain("/subtitles/download/");
        result.Content.Should().NotContain("downloadUri");
    }

    [Test]
    public void LlmFullCatalog_ReturnsTextPlainWithShows()
    {
        _tvShowRepository.GetAllHavingSubtitlesAsync().Returns(new[]
        {
            CreateShow("Breaking Bad", Guid.NewGuid()),
        }.AsQueryable());

        var result = _controller.LlmFullCatalog();

        result.Content.Should().NotBeNullOrWhiteSpace();
        result.Content.Should().Contain("/shows/");
        result.Content.Should().StartWith("# ");
        result.Content.Should().Contain("## Links");
        result.Content.Should().Contain("- [Breaking Bad](");
        result.Content.Should().NotContain("/subtitles/download/");
        result.Content.Should().NotContain("downloadUri");
    }
}
