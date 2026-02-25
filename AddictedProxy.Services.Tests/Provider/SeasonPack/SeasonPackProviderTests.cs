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

namespace AddictedProxy.Services.Tests.Provider.SeasonPack;

[TestFixture]
public class SeasonPackProviderTests
{
    private ISeasonPackSubtitleRepository _seasonPackRepo = null!;
    private ICachedStorageProvider _cachedStorageProvider = null!;
    private ISuperSubtitlesClient _superSubtitlesClient = null!;
    private ILogger<SeasonPackProvider> _logger = null!;
    private SeasonPackProvider _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _seasonPackRepo = Substitute.For<ISeasonPackSubtitleRepository>();
        _cachedStorageProvider = Substitute.For<ICachedStorageProvider>();
        _superSubtitlesClient = Substitute.For<ISuperSubtitlesClient>();
        _logger = Substitute.For<ILogger<SeasonPackProvider>>();

        _sut = new SeasonPackProvider(
            _seasonPackRepo,
            _cachedStorageProvider,
            _superSubtitlesClient,
            _logger);
    }

    private static SeasonPackSubtitle CreateSeasonPack(long externalId = 42) => new()
    {
        UniqueId = Guid.NewGuid(),
        ExternalId = externalId,
        Season = 3,
        Language = "English",
        Filename = "show.s03.en.zip"
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
}
