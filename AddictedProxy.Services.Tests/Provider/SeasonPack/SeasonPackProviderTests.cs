using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Provider.SeasonPack;
using AddictedProxy.Storage.Caching.Service;
using FluentAssertions;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SuperSubtitleClient.Generated;
using SuperSubtitleClient.Service;
using System.IO.Compression;

namespace AddictedProxy.Services.Tests.Provider.SeasonPack;

[TestFixture]
public class SeasonPackProviderTests
{
    private ISeasonPackSubtitleRepository _seasonPackRepo = null!;
    private ICachedStorageProvider _cachedStorageProvider = null!;
    private ISuperSubtitlesClient _superSubtitlesClient = null!;
    private ISeasonPackCatalogService _catalogService = null!;
    private ISeasonPackEntryRepository _entryRepository = null!;
    private ILogger<SeasonPackProvider> _logger = null!;
    private SeasonPackProvider _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _seasonPackRepo = Substitute.For<ISeasonPackSubtitleRepository>();
        _cachedStorageProvider = Substitute.For<ICachedStorageProvider>();
        _superSubtitlesClient = Substitute.For<ISuperSubtitlesClient>();
        _catalogService = Substitute.For<ISeasonPackCatalogService>();
        _entryRepository = Substitute.For<ISeasonPackEntryRepository>();
        _logger = Substitute.For<ILogger<SeasonPackProvider>>();

        _sut = new SeasonPackProvider(
            _seasonPackRepo,
            _cachedStorageProvider,
            _superSubtitlesClient,
            _catalogService,
            _entryRepository,
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

    [Test]
    public async Task GetSeasonPackFileAsync_EpisodeNotInZip_ThrowsEpisodeNotInSeasonPackException()
    {
        // Arrange
        var seasonPack = CreateSeasonPack(externalId: 99);
        var rpcException = new RpcException(new Status(StatusCode.Internal,
            $"failed to download subtitle: failed to extract episode 5 from ZIP: episode 5 {SeasonPackProvider.EpisodeNotFoundInZipDetail} (searched 13 files)"));

        _superSubtitlesClient
            .DownloadSubtitleAsync(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(rpcException);

        // Act
        var act = () => _sut.GetSeasonPackFileAsync(seasonPack, episode: 5, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EpisodeNotInSeasonPackException>()
            .Where(e => e.Episode == 5);
    }

    [Test]
    public async Task GetSeasonPackFileAsync_EpisodeFound_ReturnsStream()
    {
        // Arrange
        var seasonPack = CreateSeasonPack(externalId: 99);
        var content = new byte[] { 1, 2, 3, 4, 5 };
        var response = new DownloadSubtitleResponse { Content = ByteString.CopyFrom(content) };

        _superSubtitlesClient
            .DownloadSubtitleAsync(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<CancellationToken>())
            .Returns(response);

        // Act
        var stream = await _sut.GetSeasonPackFileAsync(seasonPack, episode: 5, CancellationToken.None);

        // Assert
        stream.Should().NotBeNull();
        var buffer = new byte[content.Length];
        await stream.ReadExactlyAsync(buffer, CancellationToken.None);
        buffer.Should().Equal(content);
    }

    [Test]
    public async Task GetSeasonPackFileAsync_NotFoundRpcException_ThrowsEpisodeNotInSeasonPackException()
    {
        // Arrange
        var seasonPack = CreateSeasonPack(externalId: 99);
        var rpcException = new RpcException(new Status(StatusCode.NotFound,
            $"episode not found in subtitle ZIP archive: failed to extract episode 55 from ZIP: episode 55 {SeasonPackProvider.EpisodeNotFoundInZipDetail} (searched 34 files)"));

        _superSubtitlesClient
            .DownloadSubtitleAsync(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(rpcException);

        // Act
        var act = () => _sut.GetSeasonPackFileAsync(seasonPack, episode: 55, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EpisodeNotInSeasonPackException>()
            .Where(e => e.Episode == 55);
    }

    [Test]
    public async Task GetSeasonPackFileAsync_OtherInternalRpcException_PropagatesOriginalException()
    {
        // Arrange
        var seasonPack = CreateSeasonPack(externalId: 99);
        var rpcException = new RpcException(new Status(StatusCode.Internal, "some unrelated internal error"));

        _superSubtitlesClient
            .DownloadSubtitleAsync(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(rpcException);

        // Act
        var act = () => _sut.GetSeasonPackFileAsync(seasonPack, episode: 5, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<RpcException>()
            .Where(e => e.StatusCode == StatusCode.Internal && e.Status.Detail == "some unrelated internal error");
    }

    #region Self-extraction tests

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
    public async Task GetSeasonPackFileAsync_StoredAndCataloged_SelfExtractsEpisode()
    {
        // Arrange
        const int episode = 3;
        const string srtContent = "1\n00:00:01,000 --> 00:00:02,000\nHello";
        var seasonPack = CreateSeasonPack(externalId: 42, storagePath: "season-pack/test.zip");
        var zipBlob = CreateZipWithEntries(("Show.S03E03.720p.srt", srtContent));

        _catalogService.IsCatalogedAsync(seasonPack.Id, Arg.Any<CancellationToken>()).Returns(true);
        _entryRepository.HasEpisodeAsync(seasonPack.Id, episode, Arg.Any<CancellationToken>()).Returns(true);
        _entryRepository.GetBySeasonPackAsync(seasonPack.Id, Arg.Any<CancellationToken>())
            .Returns(new List<SeasonPackEntry>
            {
                new() { SeasonPackSubtitleId = seasonPack.Id, EpisodeNumber = episode, FileName = "Show.S03E03.720p.srt" }
            });
        _cachedStorageProvider.GetSertAsync("season-pack", seasonPack.StoragePath!, Arg.Any<CancellationToken>())
            .Returns(new MemoryStream(zipBlob));

        // Act
        var result = await _sut.GetSeasonPackFileAsync(seasonPack, episode, CancellationToken.None);

        // Assert
        using var reader = new StreamReader(result);
        var content = await reader.ReadToEndAsync();
        content.Should().Be(srtContent);

        // Should NOT call upstream
        await _superSubtitlesClient.DidNotReceive()
            .DownloadSubtitleAsync(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetSeasonPackFileAsync_StoredCatalogedButEpisodeMissing_ThrowsEpisodeNotInSeasonPackException()
    {
        // Arrange
        const int episode = 99;
        var seasonPack = CreateSeasonPack(externalId: 42, storagePath: "season-pack/test.zip");

        _catalogService.IsCatalogedAsync(seasonPack.Id, Arg.Any<CancellationToken>()).Returns(true);
        _entryRepository.HasEpisodeAsync(seasonPack.Id, episode, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var act = () => _sut.GetSeasonPackFileAsync(seasonPack, episode, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EpisodeNotInSeasonPackException>()
            .Where(e => e.Episode == episode);
    }

    [Test]
    public async Task GetSeasonPackFileAsync_StoredButNotCataloged_FallsBackToUpstream()
    {
        // Arrange
        const int episode = 5;
        var seasonPack = CreateSeasonPack(externalId: 42, storagePath: "season-pack/test.zip");
        var content = new byte[] { 10, 20, 30 };
        var response = new DownloadSubtitleResponse { Content = ByteString.CopyFrom(content) };

        _catalogService.IsCatalogedAsync(seasonPack.Id, Arg.Any<CancellationToken>()).Returns(false);
        _superSubtitlesClient
            .DownloadSubtitleAsync("42", episode, Arg.Any<CancellationToken>())
            .Returns(response);

        // Act
        var result = await _sut.GetSeasonPackFileAsync(seasonPack, episode, CancellationToken.None);

        // Assert
        var buffer = new byte[content.Length];
        await result.ReadExactlyAsync(buffer, CancellationToken.None);
        buffer.Should().Equal(content);
    }

    [Test]
    public async Task GetSeasonPackFileAsync_NotStoredNotCataloged_FallsBackToUpstream()
    {
        // Arrange
        const int episode = 5;
        var seasonPack = CreateSeasonPack(externalId: 42, storagePath: null);
        var content = new byte[] { 10, 20, 30 };
        var response = new DownloadSubtitleResponse { Content = ByteString.CopyFrom(content) };

        _superSubtitlesClient
            .DownloadSubtitleAsync("42", episode, Arg.Any<CancellationToken>())
            .Returns(response);

        // Act
        var result = await _sut.GetSeasonPackFileAsync(seasonPack, episode, CancellationToken.None);

        // Assert
        var buffer = new byte[content.Length];
        await result.ReadExactlyAsync(buffer, CancellationToken.None);
        buffer.Should().Equal(content);

        // Should not touch catalog/entry repos since StoragePath is null
        await _catalogService.DidNotReceive().IsCatalogedAsync(Arg.Any<long>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetSeasonPackFileAsync_ZipMissingFromStorage_FallsBackToUpstream()
    {
        // Arrange
        const int episode = 3;
        var seasonPack = CreateSeasonPack(externalId: 42, storagePath: "season-pack/test.zip");
        var content = new byte[] { 7, 8, 9 };
        var response = new DownloadSubtitleResponse { Content = ByteString.CopyFrom(content) };

        _catalogService.IsCatalogedAsync(seasonPack.Id, Arg.Any<CancellationToken>()).Returns(true);
        _entryRepository.HasEpisodeAsync(seasonPack.Id, episode, Arg.Any<CancellationToken>()).Returns(true);
        _entryRepository.GetBySeasonPackAsync(seasonPack.Id, Arg.Any<CancellationToken>())
            .Returns(new List<SeasonPackEntry>
            {
                new() { SeasonPackSubtitleId = seasonPack.Id, EpisodeNumber = episode, FileName = "Show.S03E03.720p.srt" }
            });
        // Storage returns null (ZIP not found in S3)
        _cachedStorageProvider.GetSertAsync("season-pack", seasonPack.StoragePath!, Arg.Any<CancellationToken>())
            .Returns((Stream?)null);
        _superSubtitlesClient
            .DownloadSubtitleAsync("42", episode, Arg.Any<CancellationToken>())
            .Returns(response);

        // Act
        var result = await _sut.GetSeasonPackFileAsync(seasonPack, episode, CancellationToken.None);

        // Assert
        var buffer = new byte[content.Length];
        await result.ReadExactlyAsync(buffer, CancellationToken.None);
        buffer.Should().Equal(content);
    }

    #endregion
}
