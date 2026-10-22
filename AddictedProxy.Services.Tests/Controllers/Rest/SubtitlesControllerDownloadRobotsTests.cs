using AddictedProxy.Controllers.Rest;
using AddictedProxy.Culture.Model;
using AddictedProxy.Culture.Service;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Model.Api;
using AddictedProxy.Services.Provider.SeasonPack;
using AddictedProxy.Services.Provider.Subtitle;
using AddictedProxy.Services.Search;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace AddictedProxy.Services.Tests.Controllers.Rest;

/// <summary>
/// Download-wall header contract: every response of SubtitlesController.Download
/// (success and error paths alike) must carry `X-Robots-Tag: noindex, nofollow`
/// so a leaked download URL is never indexed regardless of robots.txt compliance.
/// </summary>
[TestFixture]
public class SubtitlesControllerDownloadRobotsTests
{
    private const string NoIndexNoFollow = "noindex, nofollow";

    private ISubtitleProvider _subtitleProvider = null!;
    private ICultureParser _cultureParser = null!;
    private SubtitlesController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _subtitleProvider = Substitute.For<ISubtitleProvider>();
        _cultureParser = Substitute.For<ICultureParser>();
        _controller = new SubtitlesController(
            _cultureParser,
            _subtitleProvider,
            Substitute.For<ISeasonPackProvider>(),
            Substitute.For<ISeasonPackSubtitleRepository>(),
            Substitute.For<ISearchSubtitlesService>(),
            NullLogger<SubtitlesController>.Instance
        );
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    private static Subtitle CreateSubtitle(Guid uniqueId)
    {
        return new Subtitle
        {
            UniqueId = uniqueId,
            Scene = "",
            Language = "English",
            DownloadUri = new Uri("https://www.addic7ed.com/original/1/1"),
            StoredAt = new DateTime(2024, 1, 1),
            HearingImpaired = false,
            Episode = new Episode
            {
                Title = "Pilot",
                Season = 1,
                Number = 2,
                TvShow = new TvShow { Name = "Test Show" }
            }
        };
    }

    [Test]
    public async Task Download_Success_SetsNoIndexNoFollowHeader()
    {
        var subtitleId = Guid.NewGuid();
        var subtitle = CreateSubtitle(subtitleId);
        _subtitleProvider.GetSubtitleFullAsync(subtitleId, Arg.Any<CancellationToken>())
            .Returns(subtitle);
        _subtitleProvider.GetSubtitleFileAsync(subtitle, Arg.Any<CancellationToken>())
            .Returns(new MemoryStream([1, 2, 3]));
        _cultureParser.FromStringAsync("English", Arg.Any<CancellationToken>())
            .Returns(new Culture.Model.Culture("English", "en", "en"));

        var result = await _controller.Download(Base62Id.Encode(subtitleId), CancellationToken.None);

        result.Result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.FileStreamHttpResult>();
        _controller.Response.Headers["X-Robots-Tag"].ToString().Should().Be(NoIndexNoFollow);
    }

    [Test]
    public async Task Download_SubtitleNotFound_SetsNoIndexNoFollowHeader()
    {
        var subtitleId = Guid.NewGuid();
        _subtitleProvider.GetSubtitleFullAsync(subtitleId, Arg.Any<CancellationToken>())
            .Returns((Subtitle?)null);

        var result = await _controller.Download(Base62Id.Encode(subtitleId), CancellationToken.None);

        result.Result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.NotFound<string>>();
        _controller.Response.Headers["X-Robots-Tag"].ToString().Should().Be(NoIndexNoFollow);
    }

    [Test]
    public async Task Download_InvalidSubtitleId_SetsNoIndexNoFollowHeader()
    {
        var result = await _controller.Download("not-a-valid-id", CancellationToken.None);

        result.Result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.BadRequest<AddictedProxy.Model.Responses.ErrorResponse>>();
        _controller.Response.Headers["X-Robots-Tag"].ToString().Should().Be(NoIndexNoFollow);
    }
}
