using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Model.Api;
using AddictedProxy.Model.Dto;
using FluentAssertions;

namespace AddictedProxy.Services.Tests.Model;

[TestFixture]
public class ApiIdDtoSurfaceTests
{
    /// <summary>
    /// Canonical vectors (docs/2026-10-20-api-id-base62.md): whatever shape a
    /// DTO exposes an id in, the Guid half must be the canonical base62 form.
    /// </summary>
    private static readonly Guid VectorGuid = new("1086727A-EB71-4B24-A209-7CF22374574D");
    private const string VectorBase62 = "0VBJpBmUXk2twqNJdYbHmH";
    private static readonly Guid EntryVectorGuid = new("ffffffff-ffff-ffff-ffff-ffffffffffff");
    private const string EntryVectorBase62 = "7n42DGM5Tflk9n8mt7Fhc7";

    private static Subtitle CreateSubtitle() => new()
    {
        UniqueId = VectorGuid,
        Scene = "HDTV",
        DownloadUri = new Uri("https://addic7ed.com/sub/1"),
        Language = "English",
    };

    private static SeasonPackSubtitle CreateSeasonPack() => new()
    {
        UniqueId = VectorGuid,
        Filename = "show.s01.zip",
        Language = "English",
        Source = DataSource.SuperSubtitles,
        Qualities = VideoQuality.None,
        Entries =
        [
            new SeasonPackEntry { UniqueId = EntryVectorGuid, EpisodeNumber = 3, FileName = "Show.S01E03.srt" },
        ],
    };

    [Test]
    public void SubtitleDto_RegularSubtitle_SerializesCanonicalBase62Id()
    {
        var dto = new SubtitleDto(CreateSubtitle(), "/url", null);

        dto.SubtitleId.Should().Be(VectorBase62);
        dto.SubtitleId.Should().NotContain("-", "dashed Guids are a rejected legacy form");
    }

    [Test]
    public void SeasonPackSubtitleDto_PackId_UsesSpPrefixWithCanonicalBase62()
    {
        var dto = new SeasonPackSubtitleDto(CreateSeasonPack(), "/url", null);

        dto.SubtitleId.Should().Be($"sp_{VectorBase62}");
    }

    [Test]
    public void SubtitleDto_UncatalogedPackFallback_UsesSpEpComposite()
    {
        var dto = new SubtitleDto(CreateSeasonPack(), "/url", null, episode: 3);

        dto.SubtitleId.Should().Be($"sp_{VectorBase62}_ep_3");
    }

    [Test]
    public void SubtitleDto_CatalogedPackEntry_UsesSpEntryCompositeWithBothIdsEncoded()
    {
        var dto = new SubtitleDto(CreateSeasonPack(), CreateSeasonPack().Entries.Single(), "/url", null, episode: 3);

        dto.SubtitleId.Should().Be($"sp_{VectorBase62}_entry_{EntryVectorBase62}");
    }

    [Test]
    public void SeasonPackSubtitleDto_IdsRoundTripThroughTheCodec()
    {
        // The string-format DTOs must satisfy the same round-trip contract the
        // route/JSON surfaces do: the Guid halves are rebuildable server-side.
        var dto = new SeasonPackSubtitleDto(CreateSeasonPack(), "/url", null);

        var packPart = dto.SubtitleId["sp_".Length..];
        Base62Id.TryDecode(packPart, out var decoded).Should().BeTrue($"'{packPart}' must decode");
        decoded.Should().Be(VectorGuid);
    }
}
