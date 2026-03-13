using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Model.Dto;
using FluentAssertions;

namespace AddictedProxy.Services.Tests.Provider.SeasonPack;

[TestFixture]
public class SeasonPackSubtitleDtoTests
{
    [Test]
    public void Constructor_UsesCatalogedEntriesRange_WhenProviderRangeIsMissing()
    {
        // Arrange
        var seasonPack = CreateSeasonPack();
        seasonPack.Entries =
        [
            CreateEntry(3),
            CreateEntry(8),
            CreateEntry(5)
        ];

        // Act
        var dto = new SeasonPackSubtitleDto(seasonPack, "/subtitles/download/sp_test", null);

        // Assert
        dto.RangeStart.Should().Be(3);
        dto.RangeEnd.Should().Be(8);
    }

    [Test]
    public void Constructor_KeepsProviderRange_WhenItIsPresent()
    {
        // Arrange
        var seasonPack = CreateSeasonPack();
        seasonPack.RangeStart = 2;
        seasonPack.RangeEnd = 7;
        seasonPack.Entries =
        [
            CreateEntry(3),
            CreateEntry(8)
        ];

        // Act
        var dto = new SeasonPackSubtitleDto(seasonPack, "/subtitles/download/sp_test", null);

        // Assert
        dto.RangeStart.Should().Be(2);
        dto.RangeEnd.Should().Be(7);
    }

    private static SeasonPackSubtitle CreateSeasonPack() => new()
    {
        UniqueId = Guid.NewGuid(),
        Filename = "show.s01.zip",
        Language = "English",
        Source = DataSource.SuperSubtitles,
        Qualities = VideoQuality.None,
        Entries = []
    };

    private static SeasonPackEntry CreateEntry(int episode) => new()
    {
        EpisodeNumber = episode,
        FileName = $"Show.S01E{episode:D2}.srt"
    };
}
