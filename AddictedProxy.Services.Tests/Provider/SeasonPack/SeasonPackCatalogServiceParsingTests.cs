using System.IO.Compression;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Services.Provider.SeasonPack;
using FluentAssertions;

namespace AddictedProxy.Services.Tests.Provider.SeasonPack;

[TestFixture]
public class SeasonPackCatalogServiceParsingTests
{
    private static byte[] CreateZipBlob(params string[] fileNames)
    {
        using var ms = new MemoryStream();
        using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var name in fileNames)
            {
                var entry = archive.CreateEntry(name);
                using var writer = new StreamWriter(entry.Open());
                writer.Write("dummy content");
            }
        }

        return ms.ToArray();
    }

    [Test]
    public void ParseZipEntries_SingleEpisode_ExtractsEpisodeNumber()
    {
        var blob = CreateZipBlob("Show.S01E03.srt");
        var entries = SeasonPackCatalogService.ParseZipEntries(1, blob);

        entries.Should().HaveCount(1);
        entries[0].EpisodeNumber.Should().Be(3);
        entries[0].FileName.Should().Be("Show.S01E03.srt");
        entries[0].SeasonPackSubtitleId.Should().Be(1);
    }

    [Test]
    public void ParseZipEntries_MultipleEpisodes_ParsesAll()
    {
        var blob = CreateZipBlob(
            "Show.S01E01.srt",
            "Show.S01E02.srt",
            "Show.S01E10.srt");

        var entries = SeasonPackCatalogService.ParseZipEntries(1, blob);

        entries.Should().HaveCount(3);
        entries.Select(e => e.EpisodeNumber).Should().BeEquivalentTo([1, 2, 10]);
    }

    [Test]
    public void ParseZipEntries_ThreeDigitEpisode_ParsesCorrectly()
    {
        var blob = CreateZipBlob("Show.S01E100.srt");
        var entries = SeasonPackCatalogService.ParseZipEntries(1, blob);

        entries.Should().HaveCount(1);
        entries[0].EpisodeNumber.Should().Be(100);
    }

    [Test]
    public void ParseZipEntries_CaseInsensitive_MatchesLowerAndUpperCase()
    {
        var blob = CreateZipBlob("show.s01e05.srt", "SHOW.S01E06.SRT");
        var entries = SeasonPackCatalogService.ParseZipEntries(1, blob);

        entries.Should().HaveCount(2);
        entries.Select(e => e.EpisodeNumber).Should().BeEquivalentTo([5, 6]);
    }

    [Test]
    public void ParseZipEntries_NonSubtitleFiles_AreIgnored()
    {
        var blob = CreateZipBlob(
            "Show.S01E01.srt",
            "Show.S01E02.mp4",
            "readme.txt");

        var entries = SeasonPackCatalogService.ParseZipEntries(1, blob);

        // Non-subtitle files (e.g., mp4, txt) should be ignored even if they match the SxxExx pattern
        entries.Should().HaveCount(1);
        entries.Select(e => e.FileName).Should().Contain("Show.S01E01.srt");
        entries.Select(e => e.FileName).Should().NotContain("Show.S01E02.mp4");
        entries.Select(e => e.FileName).Should().NotContain("readme.txt");
    }

    [Test]
    public void ParseZipEntries_NoMatchingFiles_ReturnsEmpty()
    {
        var blob = CreateZipBlob("readme.txt", "notes.md");
        var entries = SeasonPackCatalogService.ParseZipEntries(1, blob);

        entries.Should().BeEmpty();
    }

    [Test]
    public void ParseZipEntries_TitleAndReleaseGroup_ExtractedFromFilename()
    {
        var blob = CreateZipBlob("Show.S01E03.The.Rains.Of.Castamere.720p.WEB-DL.srt");
        var entries = SeasonPackCatalogService.ParseZipEntries(1, blob);

        entries.Should().HaveCount(1);
        entries[0].EpisodeNumber.Should().Be(3);
        entries[0].EpisodeTitle.Should().Be("The Rains Of Castamere");
        entries[0].ReleaseGroup.Should().Contain("720p");
        entries[0].ReleaseGroup.Should().Contain("WEB-DL");
    }

    [Test]
    public void ParseZipEntries_NoReleaseMarkers_TitleIncludesAllTextAfterEpisodeNumber()
    {
        var blob = CreateZipBlob("Show.S02E05.Some.Random.Story.srt");
        var entries = SeasonPackCatalogService.ParseZipEntries(1, blob);

        entries.Should().HaveCount(1);
        entries[0].EpisodeTitle.Should().Be("Some Random Story");
        entries[0].ReleaseGroup.Should().BeNull();
    }

    [Test]
    public void ParseZipEntries_MultipleReleaseMarkers_AllCaptured()
    {
        var blob = CreateZipBlob("Show.S01E01.Title.AMZN.WEB-DL.1080p.srt");
        var entries = SeasonPackCatalogService.ParseZipEntries(1, blob);

        entries.Should().HaveCount(1);
        entries[0].EpisodeTitle.Should().Be("Title");
        entries[0].ReleaseGroup.Should().Contain("AMZN");
        entries[0].ReleaseGroup.Should().Contain("WEB-DL");
        entries[0].ReleaseGroup.Should().Contain("1080p");
    }

    [Test]
    public void ParseZipEntries_StreamingServiceTag_NotIncludedInTitle()
    {
        var blob = CreateZipBlob("The.Night.Agent.S02E10.Buyers.Remorse.NF.WEB-DL.en.srt");
        var entries = SeasonPackCatalogService.ParseZipEntries(1, blob);

        entries.Should().HaveCount(1);
        entries[0].EpisodeNumber.Should().Be(10);
        entries[0].EpisodeTitle.Should().Be("Buyers Remorse");
        entries[0].ReleaseGroup.Should().Contain("NF");
        entries[0].ReleaseGroup.Should().Contain("WEB-DL");
    }

    [Test]
    public void ParseZipEntries_SubFolders_FullNameUsed()
    {
        var blob = CreateZipBlob("Season 1/Show.S01E01.srt");
        var entries = SeasonPackCatalogService.ParseZipEntries(1, blob);

        entries.Should().HaveCount(1);
        entries[0].FileName.Should().Be("Season 1/Show.S01E01.srt");
    }

    [Test]
    public void ParseZipEntries_AllSubtitleExtensions_Matched()
    {
        var blob = CreateZipBlob(
            "Show.S01E01.srt",
            "Show.S01E02.sub",
            "Show.S01E03.ass",
            "Show.S01E04.ssa",
            "Show.S01E05.idx");

        var entries = SeasonPackCatalogService.ParseZipEntries(1, blob);

        entries.Should().HaveCount(5);
        entries.Select(e => e.EpisodeNumber).Should().BeEquivalentTo([1, 2, 3, 4, 5]);
    }

    [Test]
    public void ParseZipEntries_EmptyZip_ReturnsEmpty()
    {
        var blob = CreateZipBlob();
        var entries = SeasonPackCatalogService.ParseZipEntries(1, blob);

        entries.Should().BeEmpty();
    }
}
