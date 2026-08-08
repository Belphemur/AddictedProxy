using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Services.Provider.SuperSubtitles;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

using ProtoSubtitle = SuperSubtitleClient.Generated.Subtitle;

namespace AddictedProxy.Services.Tests.Provider.SuperSubtitles;

[TestFixture]
public class SuperSubtitlesStreamFilterTests
{
    private ILogger _logger = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger>();
    }

    [Test]
    public void DropInvalidSeasons_KeepsSpecialsAndValidSeasons_DropsBeyondTmdbCount()
    {
        // Arrange
        var show = CreateShow(numberOfSeasons: 1);
        var subtitles = new[]
        {
            new ProtoSubtitle { Season = 0, Episode = 1 },
            new ProtoSubtitle { Season = 1, Episode = 1 },
            new ProtoSubtitle { Season = 5, Episode = 1 },
            new ProtoSubtitle { Season = 8, Episode = 1 },
            new ProtoSubtitle { Season = 9, Episode = 1 }
        };

        // Act
        var result = SuperSubtitlesStreamFilter.DropInvalidSeasons(show, subtitles, _logger);

        // Assert
        result.Select(s => s.Season).Should().Equal(0, 1);
    }

    [Test]
    public void DropInvalidSeasons_UnknownSeasonCount_KeepsEverything()
    {
        // Arrange
        var show = CreateShow(numberOfSeasons: null);
        var subtitles = new[]
        {
            new ProtoSubtitle { Season = 1, Episode = 1 },
            new ProtoSubtitle { Season = 5, Episode = 1 },
            new ProtoSubtitle { Season = 9, Episode = 1 }
        };

        // Act
        var result = SuperSubtitlesStreamFilter.DropInvalidSeasons(show, subtitles, _logger);

        // Assert
        result.Select(s => s.Season).Should().Equal(1, 5, 9);
    }

    [Test]
    public void DropInvalidSeasons_SeasonEqualToCount_IsKept()
    {
        // Arrange
        var show = CreateShow(numberOfSeasons: 2);
        var subtitles = new[]
        {
            new ProtoSubtitle { Season = 1, Episode = 1 },
            new ProtoSubtitle { Season = 2, Episode = 1 },
            new ProtoSubtitle { Season = 3, Episode = 1 }
        };

        // Act
        var result = SuperSubtitlesStreamFilter.DropInvalidSeasons(show, subtitles, _logger);

        // Assert
        result.Select(s => s.Season).Should().Equal(1, 2);
    }

    [Test]
    public void DropInvalidSeasons_NothingDropped_ReturnsAll()
    {
        // Arrange
        var show = CreateShow(numberOfSeasons: 10);
        var subtitles = new[]
        {
            new ProtoSubtitle { Season = 1, Episode = 1 },
            new ProtoSubtitle { Season = 2, Episode = 1 },
            new ProtoSubtitle { Season = 3, Episode = 1 }
        };

        // Act
        var result = SuperSubtitlesStreamFilter.DropInvalidSeasons(show, subtitles, _logger);

        // Assert
        result.Select(s => s.Season).Should().Equal(1, 2, 3);
    }

    private static TvShow CreateShow(int? numberOfSeasons)
    {
        return new TvShow { Name = "The Shards", NumberOfSeasons = numberOfSeasons };
    }
}
