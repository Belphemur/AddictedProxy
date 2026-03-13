using System.IO.Compression;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Provider.SeasonPack;
using AddictedProxy.Storage.Caching.Service;
using FluentAssertions;
using Google.Protobuf;
using Grpc.Core;
using Hangfire;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SuperSubtitleClient.Generated;
using SuperSubtitleClient.Service;

namespace AddictedProxy.Services.Tests.Provider.SeasonPack;

[TestFixture]
public class SeasonPackProviderTests
{
    private ISeasonPackSubtitleRepository _seasonPackRepo = null!;
    private ICachedStorageProvider _cachedStorageProvider = null!;
    private ISuperSubtitlesClient _superSubtitlesClient = null!;
    private ISeasonPackEntryRepository _entryRepository = null!;
    private IBackgroundJobClient _backgroundJobClient = null!;
    private ILogger<SeasonPackProvider> _logger = null!;
    private SeasonPackProvider _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _seasonPackRepo = Substitute.For<ISeasonPackSubtitleRepository>();
        _cachedStorageProvider = Substitute.For<ICachedStorageProvider>();
        _superSubtitlesClient = Substitute.For<ISuperSubtitlesClient>();
        _entryRepository = Substitute.For<ISeasonPackEntryRepository>();
        _backgroundJobClient = Substitute.For<IBackgroundJobClient>();
        _logger = Substitute.For<ILogger<SeasonPackProvider>>();

        _sut = new SeasonPackProvider(
            _seasonPackRepo,
            _cachedStorageProvider,
            _superSubtitlesClient,
            _entryRepository,
            _backgroundJobClient,
            _logger);
    }

    private static SeasonPackSubtitle CreateSeasonPack(long externalId = 42, string? storagePath = null) => new()
    {
        Id = 1,
        UniqueId = Guid.NewGuid(),
        ExternalId = externalId,
        Season = 3,
        Language = "English",
        Filename = "show.s03.en.zip",
        StoragePath = storagePath,
        StoredAt = storagePath != null ? DateTime.UtcNow : null
    };

    private static SeasonPackEntry CreateEntry(long seasonPackId, int episodeNumber, string fileName) => new()
    {
        Id = 1,
        UniqueId = Guid.NewGuid(),
        SeasonPackSubtitleId = seasonPackId,
        EpisodeNumber = episodeNumber,
        FileName = fileName,
        EpisodeTitle = "Test Episode"
    };

    #region GetEpisodeFromUpstreamAsync tests

    [Test]
    public async Task GetEpisodeFromUpstreamAsync_EpisodeNotInZip_ThrowsEpisodeNotInSeasonPackException()
    {
        // Arrange
        var seasonPack = CreateSeasonPack(externalId: 99);
        var rpcException = new RpcException(new Status(StatusCode.NotFound,
            $"failed to download subtitle: failed to extract episode 5 from archive: episode 5 (searched 13 files)"));

        _superSubtitlesClient
            .DownloadSubtitleAsync(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(rpcException);

        // Act
        var act = () => _sut.GetEpisodeFromUpstreamAsync(seasonPack, 5, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EpisodeNotInSeasonPackException>()
            .Where(e => e.Episode == 5);
    }

    [Test]
    public async Task GetEpisodeFromUpstreamAsync_EpisodeFound_ReturnsStream()
    {
        // Arrange
        var seasonPack = CreateSeasonPack(externalId: 99);
        var content = new byte[] { 1, 2, 3, 4, 5 };
        var response = new DownloadSubtitleResponse { Content = ByteString.CopyFrom(content) };

        _superSubtitlesClient
            .DownloadSubtitleAsync(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<CancellationToken>())
            .Returns(response);

        // Act
        var stream = await _sut.GetEpisodeFromUpstreamAsync(seasonPack, 5, CancellationToken.None);

        // Assert
        stream.Should().NotBeNull();
        var buffer = new byte[content.Length];
        await stream.ReadExactlyAsync(buffer, CancellationToken.None);
        buffer.Should().Equal(content);
    }

    [Test]
    public async Task GetEpisodeFromUpstreamAsync_NotFoundRpcException_ThrowsEpisodeNotInSeasonPackException()
    {
        // Arrange
        var seasonPack = CreateSeasonPack(externalId: 99);
        var rpcException = new RpcException(new Status(StatusCode.NotFound,
            $"episode not found in subtitle ZIP archive: failed to extract episode 55 from archive: episode 55 (searched 34 files)"));

        _superSubtitlesClient
            .DownloadSubtitleAsync(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(rpcException);

        // Act
        var act = () => _sut.GetEpisodeFromUpstreamAsync(seasonPack, 55, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EpisodeNotInSeasonPackException>()
            .Where(e => e.Episode == 55);
    }

    [Test]
    public async Task GetEpisodeFromUpstreamAsync_OtherInternalRpcException_PropagatesOriginalException()
    {
        // Arrange
        var seasonPack = CreateSeasonPack(externalId: 99);
        var rpcException = new RpcException(new Status(StatusCode.Internal, "some unrelated internal error"));

        _superSubtitlesClient
            .DownloadSubtitleAsync(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(rpcException);

        // Act
        var act = () => _sut.GetEpisodeFromUpstreamAsync(seasonPack, 5, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<RpcException>()
            .Where(e => e.StatusCode == StatusCode.Internal && e.Status.Detail == "some unrelated internal error");
    }

    [Test]
    public async Task GetEpisodeFromUpstreamAsync_EpisodeBeforeKnownRange_ThrowsWithoutCallingUpstream()
    {
        // Arrange
        var seasonPack = CreateSeasonPack(externalId: 99);
        seasonPack.RangeStart = 3;
        seasonPack.RangeEnd = 8;

        // Act
        var act = () => _sut.GetEpisodeFromUpstreamAsync(seasonPack, 2, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EpisodeNotInSeasonPackException>()
            .Where(e => e.Episode == 2);

        await _superSubtitlesClient.DidNotReceive()
            .DownloadSubtitleAsync(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetEpisodeFromUpstreamAsync_EpisodeAfterKnownRange_ThrowsWithoutCallingUpstream()
    {
        // Arrange
        var seasonPack = CreateSeasonPack(externalId: 99);
        seasonPack.RangeStart = 3;
        seasonPack.RangeEnd = 8;

        // Act
        var act = () => _sut.GetEpisodeFromUpstreamAsync(seasonPack, 9, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EpisodeNotInSeasonPackException>()
            .Where(e => e.Episode == 9);

        await _superSubtitlesClient.DidNotReceive()
            .DownloadSubtitleAsync(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region GetEntryFileAsync tests

    private static byte[] CreateZipWithEntries(params (string name, string content)[] entries)
    {
        using var ms = new MemoryStream();
        using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var (name, content) in entries)
            {
                var entry = archive.CreateEntry(name);
                using var writer = new StreamWriter(entry.Open());
                writer.Write(content);
            }
        }

        return ms.ToArray();
    }

    [Test]
    public async Task GetEntryFileAsync_StoredAndCataloged_SelfExtractsEntry()
    {
        // Arrange
        const int episode = 3;
        const string srtContent = "1\n00:00:01,000 --> 00:00:02,000\nHello";
        var seasonPack = CreateSeasonPack(externalId: 42, storagePath: "season-pack/test.zip");
        var entry = CreateEntry(seasonPack.Id, episode, "Show.S03E03.720p.srt");
        var zipBlob = CreateZipWithEntries(("Show.S03E03.720p.srt", srtContent));

        _cachedStorageProvider.GetSertAsync("season-pack", seasonPack.StoragePath!, Arg.Any<CancellationToken>())
            .Returns(new MemoryStream(zipBlob));

        // Act
        var result = await _sut.GetEntryFileAsync(seasonPack, entry, CancellationToken.None);

        // Assert
        using var reader = new StreamReader(result);
        var content = await reader.ReadToEndAsync();
        content.Should().Be(srtContent);

        // Should NOT call upstream
        await _superSubtitlesClient.DidNotReceive()
            .DownloadSubtitleAsync(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetEntryFileAsync_NoStoragePath_FallsBackToUpstream()
    {
        // Arrange
        const int episode = 5;
        var seasonPack = CreateSeasonPack(externalId: 42, storagePath: null);
        var entry = CreateEntry(seasonPack.Id, episode, "Show.S03E05.720p.srt");
        var content = new byte[] { 10, 20, 30 };
        var response = new DownloadSubtitleResponse { Content = ByteString.CopyFrom(content) };

        _superSubtitlesClient
            .DownloadSubtitleAsync("42", episode, Arg.Any<CancellationToken>())
            .Returns(response);

        // Act
        var result = await _sut.GetEntryFileAsync(seasonPack, entry, CancellationToken.None);

        // Assert
        var buffer = new byte[content.Length];
        await result.ReadExactlyAsync(buffer, CancellationToken.None);
        buffer.Should().Equal(content);
    }

    [Test]
    public async Task GetEntryFileAsync_ZipMissingFromStorage_FallsBackToUpstream()
    {
        // Arrange
        const int episode = 3;
        var seasonPack = CreateSeasonPack(externalId: 42, storagePath: "season-pack/test.zip");
        var entry = CreateEntry(seasonPack.Id, episode, "Show.S03E03.720p.srt");
        var content = new byte[] { 7, 8, 9 };
        var response = new DownloadSubtitleResponse { Content = ByteString.CopyFrom(content) };

        // Storage returns null (ZIP not found in S3)
        _cachedStorageProvider.GetSertAsync("season-pack", seasonPack.StoragePath!, Arg.Any<CancellationToken>())
            .Returns((Stream?)null);
        _superSubtitlesClient
            .DownloadSubtitleAsync("42", episode, Arg.Any<CancellationToken>())
            .Returns(response);

        // Act
        var result = await _sut.GetEntryFileAsync(seasonPack, entry, CancellationToken.None);

        // Assert
        var buffer = new byte[content.Length];
        await result.ReadExactlyAsync(buffer, CancellationToken.None);
        buffer.Should().Equal(content);
    }

    [Test]
    public async Task GetEntryFileAsync_EntryNotInZip_FallsBackToUpstream()
    {
        // Arrange
        const int episode = 3;
        var seasonPack = CreateSeasonPack(externalId: 42, storagePath: "season-pack/test.zip");
        var entry = CreateEntry(seasonPack.Id, episode, "Show.S03E03.720p.srt");
        var zipBlob = CreateZipWithEntries(("Show.S03E04.720p.srt", "wrong episode"));
        var content = new byte[] { 7, 8, 9 };
        var response = new DownloadSubtitleResponse { Content = ByteString.CopyFrom(content) };

        _cachedStorageProvider.GetSertAsync("season-pack", seasonPack.StoragePath!, Arg.Any<CancellationToken>())
            .Returns(new MemoryStream(zipBlob));
        _superSubtitlesClient
            .DownloadSubtitleAsync("42", episode, Arg.Any<CancellationToken>())
            .Returns(response);

        // Act
        var result = await _sut.GetEntryFileAsync(seasonPack, entry, CancellationToken.None);

        // Assert
        var buffer = new byte[content.Length];
        await result.ReadExactlyAsync(buffer, CancellationToken.None);
        buffer.Should().Equal(content);
    }

    #endregion

    #region GetSeasonPackZipAsync tests

    [Test]
    public async Task GetSeasonPackZipAsync_StoredInStorage_ReturnsFromStorage()
    {
        // Arrange
        var seasonPack = CreateSeasonPack(externalId: 42, storagePath: "season-pack/test.zip");
        var zipContent = new byte[] { 1, 2, 3 };

        _cachedStorageProvider.GetSertAsync("season-pack", seasonPack.StoragePath!, Arg.Any<CancellationToken>())
            .Returns(new MemoryStream(zipContent));

        // Act
        var result = await _sut.GetSeasonPackZipAsync(seasonPack, CancellationToken.None);

        // Assert
        var buffer = new byte[zipContent.Length];
        await result.ReadExactlyAsync(buffer, CancellationToken.None);
        buffer.Should().Equal(zipContent);

        await _superSubtitlesClient.DidNotReceive()
            .DownloadSubtitleAsync(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetSeasonPackZipAsync_NotStored_DownloadsFromUpstream()
    {
        // Arrange
        var seasonPack = CreateSeasonPack(externalId: 42, storagePath: null);
        var content = new byte[] { 10, 20, 30 };
        var response = new DownloadSubtitleResponse { Content = ByteString.CopyFrom(content) };

        _superSubtitlesClient
            .DownloadSubtitleAsync("42", null, Arg.Any<CancellationToken>())
            .Returns(response);

        // Act
        var result = await _sut.GetSeasonPackZipAsync(seasonPack, CancellationToken.None);

        // Assert
        var buffer = new byte[content.Length];
        await result.ReadExactlyAsync(buffer, CancellationToken.None);
        buffer.Should().Equal(content);
    }

    #endregion
}
