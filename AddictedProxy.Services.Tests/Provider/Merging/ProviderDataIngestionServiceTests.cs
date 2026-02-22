using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Provider.Merging;
using AddictedProxy.Services.Provider.Merging.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using TvMovieDatabaseClient.Model.Mapping;
using TvMovieDatabaseClient.Model.Movie.Search;
using TvMovieDatabaseClient.Model.Show.Search;
using TvMovieDatabaseClient.Service;

namespace AddictedProxy.Services.Tests.Provider.Merging;

[TestFixture]
public class ProviderDataIngestionServiceTests
{
    private IShowExternalIdRepository _showExternalIdRepo = null!;
    private ITvShowRepository _tvShowRepo = null!;
    private IEpisodeRepository _episodeRepo = null!;
    private ISeasonRepository _seasonRepo = null!;
    private ISeasonPackSubtitleRepository _seasonPackRepo = null!;
    private ITMDBClient _tmdbClient = null!;
    private ILogger<ProviderDataIngestionService> _logger = null!;
    private ProviderDataIngestionService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _showExternalIdRepo = Substitute.For<IShowExternalIdRepository>();
        _tvShowRepo = Substitute.For<ITvShowRepository>();
        _episodeRepo = Substitute.For<IEpisodeRepository>();
        _seasonRepo = Substitute.For<ISeasonRepository>();
        _seasonPackRepo = Substitute.For<ISeasonPackSubtitleRepository>();
        _tmdbClient = Substitute.For<ITMDBClient>();
        _logger = Substitute.For<ILogger<ProviderDataIngestionService>>();

        _sut = new ProviderDataIngestionService(
            _showExternalIdRepo,
            _tvShowRepo,
            _episodeRepo,
            _seasonRepo,
            _seasonPackRepo,
            _tmdbClient,
            _logger);
    }

    #region MergeShowAsync — Fast path (ShowExternalId lookup)

    [Test]
    public async Task MergeShowAsync_AlreadyImported_ReturnsExistingShow()
    {
        // Arrange
        var existingShow = CreateTvShow(id: 42, name: "Breaking Bad", tvdbId: 81189, tmdbId: 1396);
        var externalId = new ShowExternalId
        {
            TvShowId = 42,
            TvShow = existingShow,
            Source = DataSource.SuperSubtitles,
            ExternalId = "100"
        };

        _showExternalIdRepo
            .GetBySourceAndExternalIdAsync(DataSource.SuperSubtitles, "100", Arg.Any<CancellationToken>())
            .Returns(externalId);

        // Act
        var result = await _sut.MergeShowAsync(
            DataSource.SuperSubtitles, "100", "Breaking Bad",
            new ThirdPartyShowIds(81189, null, 1396),
            CancellationToken.None);

        // Assert
        result.Should().BeSameAs(existingShow);
        _tvShowRepo.DidNotReceive().GetByTvdbIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _tvShowRepo.DidNotReceive().InsertShowAsync(Arg.Any<TvShow>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task MergeShowAsync_AlreadyImported_BackfillsMissingTvdbId()
    {
        // Arrange — existing show has TMDB but not TvDB
        var existingShow = CreateTvShow(id: 42, name: "Breaking Bad", tvdbId: null, tmdbId: 1396);
        var externalId = new ShowExternalId
        {
            TvShowId = 42,
            TvShow = existingShow,
            Source = DataSource.SuperSubtitles,
            ExternalId = "100"
        };

        _showExternalIdRepo
            .GetBySourceAndExternalIdAsync(DataSource.SuperSubtitles, "100", Arg.Any<CancellationToken>())
            .Returns(externalId);

        // Act
        await _sut.MergeShowAsync(
            DataSource.SuperSubtitles, "100", "Breaking Bad",
            new ThirdPartyShowIds(81189, null, 1396),
            CancellationToken.None);

        // Assert — TvDB ID should be backfilled
        existingShow.TvdbId.Should().Be(81189);
        await _tvShowRepo.Received(1).UpdateShowAsync(existingShow, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task MergeShowAsync_AlreadyImported_NoBackfillWhenIdsPresent()
    {
        // Arrange — existing show already has all IDs
        var existingShow = CreateTvShow(id: 42, name: "Breaking Bad", tvdbId: 81189, tmdbId: 1396);
        var externalId = new ShowExternalId
        {
            TvShowId = 42,
            TvShow = existingShow,
            Source = DataSource.SuperSubtitles,
            ExternalId = "100"
        };

        _showExternalIdRepo
            .GetBySourceAndExternalIdAsync(DataSource.SuperSubtitles, "100", Arg.Any<CancellationToken>())
            .Returns(externalId);

        // Act
        await _sut.MergeShowAsync(
            DataSource.SuperSubtitles, "100", "Breaking Bad",
            new ThirdPartyShowIds(81189, null, 1396),
            CancellationToken.None);

        // Assert — no update needed
        await _tvShowRepo.DidNotReceive().UpdateShowAsync(Arg.Any<TvShow>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region MergeShowAsync — Match by TvDB ID

    [Test]
    public async Task MergeShowAsync_MatchByTvdbId_ReturnsMatchedShow()
    {
        // Arrange
        _showExternalIdRepo
            .GetBySourceAndExternalIdAsync(DataSource.SuperSubtitles, "100", Arg.Any<CancellationToken>())
            .ReturnsNull();

        var existingShow = CreateTvShow(id: 42, name: "Breaking Bad", tvdbId: 81189, tmdbId: null);
        _tvShowRepo
            .GetByTvdbIdAsync(81189, Arg.Any<CancellationToken>())
            .Returns(new[] { existingShow }.ToAsyncEnumerable());

        // Act
        var result = await _sut.MergeShowAsync(
            DataSource.SuperSubtitles, "100", "Breaking Bad",
            new ThirdPartyShowIds(81189, null, 1396),
            CancellationToken.None);

        // Assert
        result.Should().BeSameAs(existingShow);
        await _showExternalIdRepo.Received(1).UpsertAsync(
            Arg.Is<ShowExternalId>(e =>
                e.TvShowId == 42 &&
                e.Source == DataSource.SuperSubtitles &&
                e.ExternalId == "100"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task MergeShowAsync_MatchByTvdbId_BackfillsTmdbId()
    {
        // Arrange
        _showExternalIdRepo
            .GetBySourceAndExternalIdAsync(DataSource.SuperSubtitles, "100", Arg.Any<CancellationToken>())
            .ReturnsNull();

        var existingShow = CreateTvShow(id: 42, name: "Breaking Bad", tvdbId: 81189, tmdbId: null);
        _tvShowRepo
            .GetByTvdbIdAsync(81189, Arg.Any<CancellationToken>())
            .Returns(new[] { existingShow }.ToAsyncEnumerable());

        // Act
        await _sut.MergeShowAsync(
            DataSource.SuperSubtitles, "100", "Breaking Bad",
            new ThirdPartyShowIds(81189, null, 1396),
            CancellationToken.None);

        // Assert — TMDB ID should be backfilled
        existingShow.TmdbId.Should().Be(1396);
        await _tvShowRepo.Received(1).UpdateShowAsync(existingShow, Arg.Any<CancellationToken>());
    }

    #endregion

    #region MergeShowAsync — Match by TMDB ID

    [Test]
    public async Task MergeShowAsync_MatchByTmdbId_ReturnsMatchedShow()
    {
        // Arrange
        _showExternalIdRepo
            .GetBySourceAndExternalIdAsync(DataSource.SuperSubtitles, "100", Arg.Any<CancellationToken>())
            .ReturnsNull();

        _tvShowRepo
            .GetByTvdbIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(AsyncEnumerable.Empty<TvShow>());

        var existingShow = CreateTvShow(id: 42, name: "Breaking Bad", tvdbId: null, tmdbId: 1396);
        _tvShowRepo
            .GetShowsByTmdbIdAsync(1396)
            .Returns(new[] { existingShow }.ToAsyncEnumerable());

        // Act
        var result = await _sut.MergeShowAsync(
            DataSource.SuperSubtitles, "100", "Breaking Bad",
            new ThirdPartyShowIds(null, null, 1396),
            CancellationToken.None);

        // Assert
        result.Should().BeSameAs(existingShow);
        await _showExternalIdRepo.Received(1).UpsertAsync(
            Arg.Is<ShowExternalId>(e => e.TvShowId == 42 && e.Source == DataSource.SuperSubtitles),
            Arg.Any<CancellationToken>());
    }

    #endregion

    #region MergeShowAsync — Match by IMDB → TMDB

    [Test]
    public async Task MergeShowAsync_MatchByImdbToTmdb_TvResult_ReturnsMatchedShow()
    {
        // Arrange
        _showExternalIdRepo
            .GetBySourceAndExternalIdAsync(DataSource.SuperSubtitles, "100", Arg.Any<CancellationToken>())
            .ReturnsNull();

        // No TvDB or TMDB match
        _tvShowRepo
            .GetByTvdbIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(AsyncEnumerable.Empty<TvShow>());
        _tvShowRepo
            .GetShowsByTmdbIdAsync(Arg.Any<int>())
            .Returns(AsyncEnumerable.Empty<TvShow>());

        // IMDB → TMDB lookup returns a TV result
        _tmdbClient
            .FindByExternalIdAsync("tt0903747", "imdb_id", Arg.Any<CancellationToken>())
            .Returns(new FindByExternalIdResult
            {
                TvResults = [new ShowSearchResult { Id = 1396, Name = "Breaking Bad" }],
                MovieResults = []
            });

        // After resolving TMDB ID, match by TMDB (step 3 is skipped since TmdbId is null,
        // so this is only called once in step 4 after IMDB resolution)
        var existingShow = CreateTvShow(id: 42, name: "Breaking Bad", tvdbId: 81189, tmdbId: 1396);
        _tvShowRepo
            .GetShowsByTmdbIdAsync(1396)
            .Returns(new[] { existingShow }.ToAsyncEnumerable());

        // Act
        var result = await _sut.MergeShowAsync(
            DataSource.SuperSubtitles, "100", "Breaking Bad",
            new ThirdPartyShowIds(null, "tt0903747", null),
            CancellationToken.None);

        // Assert
        result.Should().BeSameAs(existingShow);
    }

    [Test]
    public async Task MergeShowAsync_MatchByImdbToTmdb_MovieResult_ReturnsMatchedShow()
    {
        // Arrange
        _showExternalIdRepo
            .GetBySourceAndExternalIdAsync(DataSource.SuperSubtitles, "200", Arg.Any<CancellationToken>())
            .ReturnsNull();

        // IMDB → TMDB returns a movie result
        _tmdbClient
            .FindByExternalIdAsync("tt1375666", "imdb_id", Arg.Any<CancellationToken>())
            .Returns(new FindByExternalIdResult
            {
                TvResults = [],
                MovieResults = [new MovieSearchResult { Id = 27205, Title = "Inception" }]
            });

        var existingShow = CreateTvShow(id: 99, name: "Inception", tvdbId: null, tmdbId: 27205);
        _tvShowRepo
            .GetShowsByTmdbIdAsync(27205)
            .Returns(new[] { existingShow }.ToAsyncEnumerable());

        // Act
        var result = await _sut.MergeShowAsync(
            DataSource.SuperSubtitles, "200", "Inception",
            new ThirdPartyShowIds(null, "tt1375666", null),
            CancellationToken.None);

        // Assert
        result.Should().BeSameAs(existingShow);
    }

    [Test]
    public async Task MergeShowAsync_ImdbLookupReturnsNull_CreatesNewShow()
    {
        // Arrange — TMDB find returns null (API error)
        _showExternalIdRepo
            .GetBySourceAndExternalIdAsync(DataSource.SuperSubtitles, "300", Arg.Any<CancellationToken>())
            .ReturnsNull();

        _tmdbClient
            .FindByExternalIdAsync("tt9999999", "imdb_id", Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await _sut.MergeShowAsync(
            DataSource.SuperSubtitles, "300", "Unknown Show",
            new ThirdPartyShowIds(null, "tt9999999", null),
            CancellationToken.None);

        // Assert — new show created
        await _tvShowRepo.Received(1).InsertShowAsync(
            Arg.Is<TvShow>(s =>
                s.Name == "Unknown Show" &&
                s.Source == DataSource.SuperSubtitles),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task MergeShowAsync_ImdbLookupThrows_CreatesNewShow()
    {
        // Arrange — TMDB find throws exception
        _showExternalIdRepo
            .GetBySourceAndExternalIdAsync(DataSource.SuperSubtitles, "400", Arg.Any<CancellationToken>())
            .ReturnsNull();

        _tmdbClient
            .FindByExternalIdAsync("tt8888888", "imdb_id", Arg.Any<CancellationToken>())
            .Throws(new HttpRequestException("TMDB API unavailable"));

        // Act
        var result = await _sut.MergeShowAsync(
            DataSource.SuperSubtitles, "400", "Some Show",
            new ThirdPartyShowIds(null, "tt8888888", null),
            CancellationToken.None);

        // Assert — should not throw; creates new show
        result.Should().NotBeNull();
        result.Name.Should().Be("Some Show");
        await _tvShowRepo.Received(1).InsertShowAsync(Arg.Any<TvShow>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region MergeShowAsync — Create new show

    [Test]
    public async Task MergeShowAsync_NoMatch_CreatesNewShowWithCorrectProperties()
    {
        // Arrange
        _showExternalIdRepo
            .GetBySourceAndExternalIdAsync(DataSource.SuperSubtitles, "500", Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await _sut.MergeShowAsync(
            DataSource.SuperSubtitles, "500", "New Show",
            new ThirdPartyShowIds(12345, null, 6789),
            CancellationToken.None);

        // Assert
        await _tvShowRepo.Received(1).InsertShowAsync(
            Arg.Is<TvShow>(s =>
                s.Name == "New Show" &&
                s.Source == DataSource.SuperSubtitles &&
                s.TvdbId == 12345 &&
                s.TmdbId == 6789),
            Arg.Any<CancellationToken>());

        await _showExternalIdRepo.Received(1).UpsertAsync(
            Arg.Is<ShowExternalId>(e =>
                e.Source == DataSource.SuperSubtitles &&
                e.ExternalId == "500"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task MergeShowAsync_NoThirdPartyIds_CreatesNewShow()
    {
        // Arrange
        _showExternalIdRepo
            .GetBySourceAndExternalIdAsync(DataSource.SuperSubtitles, "600", Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await _sut.MergeShowAsync(
            DataSource.SuperSubtitles, "600", "Show Without IDs",
            null,
            CancellationToken.None);

        // Assert
        await _tvShowRepo.Received(1).InsertShowAsync(
            Arg.Is<TvShow>(s =>
                s.Name == "Show Without IDs" &&
                s.Source == DataSource.SuperSubtitles &&
                s.TvdbId == null &&
                s.TmdbId == null),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task MergeShowAsync_ThirdPartyIdsAllZeroOrNull_CreatesNewShow()
    {
        // Arrange — TvDB and TMDB IDs are 0 (which should be treated as absent)
        _showExternalIdRepo
            .GetBySourceAndExternalIdAsync(DataSource.SuperSubtitles, "700", Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await _sut.MergeShowAsync(
            DataSource.SuperSubtitles, "700", "Show With Zero IDs",
            new ThirdPartyShowIds(0, null, 0),
            CancellationToken.None);

        // Assert — should not attempt TvDB or TMDB lookups
        _tvShowRepo.DidNotReceive().GetByTvdbIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
        _tvShowRepo.DidNotReceive().GetShowsByTmdbIdAsync(Arg.Any<int>());
        await _tvShowRepo.Received(1).InsertShowAsync(Arg.Any<TvShow>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region MergeShowAsync — Backfill edge cases

    [Test]
    public async Task MergeShowAsync_MatchByTvdb_BackfillsTmdbFromImdbResolve()
    {
        // Arrange — show matched by TvDB, but IMDB path was not needed.
        // The show has TvDB but no TMDB.
        _showExternalIdRepo
            .GetBySourceAndExternalIdAsync(DataSource.SuperSubtitles, "800", Arg.Any<CancellationToken>())
            .ReturnsNull();

        var existingShow = CreateTvShow(id: 50, name: "Test Show", tvdbId: 999, tmdbId: null);
        _tvShowRepo
            .GetByTvdbIdAsync(999, Arg.Any<CancellationToken>())
            .Returns(new[] { existingShow }.ToAsyncEnumerable());

        // Act
        await _sut.MergeShowAsync(
            DataSource.SuperSubtitles, "800", "Test Show",
            new ThirdPartyShowIds(999, null, 555),
            CancellationToken.None);

        // Assert — TMDB should be backfilled
        existingShow.TmdbId.Should().Be(555);
        await _tvShowRepo.Received(1).UpdateShowAsync(existingShow, Arg.Any<CancellationToken>());
    }

    #endregion

    #region IngestSeasonPackAsync

    [Test]
    public async Task IngestSeasonPackAsync_CallsBulkUpsert()
    {
        // Arrange
        var seasonPack = new SeasonPackSubtitle
        {
            TvShowId = 1,
            Season = 2,
            Source = DataSource.SuperSubtitles,
            ExternalId = 12345,
            Filename = "show.s02.srt",
            Language = "English",
            Discovered = DateTime.UtcNow
        };

        // Act
        await _sut.IngestSeasonPackAsync(seasonPack, CancellationToken.None);

        // Assert
        await _seasonPackRepo.Received(1).BulkUpsertAsync(
            Arg.Is<IEnumerable<SeasonPackSubtitle>>(packs =>
                packs.Any(p =>
                    p.TvShowId == 1 &&
                    p.Season == 2 &&
                    p.ExternalId == 12345)),
            Arg.Any<CancellationToken>());
    }

    #endregion

    #region MergeShowAsync — Priority / lookup order

    [Test]
    public async Task MergeShowAsync_TvdbMatchTakesPrecedenceOverTmdb()
    {
        // Arrange — both TvDB and TMDB IDs provided, but different shows would match
        _showExternalIdRepo
            .GetBySourceAndExternalIdAsync(DataSource.SuperSubtitles, "900", Arg.Any<CancellationToken>())
            .ReturnsNull();

        var tvdbMatched = CreateTvShow(id: 10, name: "TvDB Show", tvdbId: 111);
        _tvShowRepo
            .GetByTvdbIdAsync(111, Arg.Any<CancellationToken>())
            .Returns(new[] { tvdbMatched }.ToAsyncEnumerable());

        var tmdbMatched = CreateTvShow(id: 20, name: "TMDB Show", tmdbId: 222);
        _tvShowRepo
            .GetShowsByTmdbIdAsync(222)
            .Returns(new[] { tmdbMatched }.ToAsyncEnumerable());

        // Act
        var result = await _sut.MergeShowAsync(
            DataSource.SuperSubtitles, "900", "Some Show",
            new ThirdPartyShowIds(111, null, 222),
            CancellationToken.None);

        // Assert — TvDB match should win
        result.Should().BeSameAs(tvdbMatched);
        // TMDB lookup should not be called (TvDB already matched)
        _tvShowRepo.DidNotReceive().GetShowsByTmdbIdAsync(222);
    }

    [Test]
    public async Task MergeShowAsync_ExternalIdLookupTakesPrecedenceOverTvdb()
    {
        // Arrange — ShowExternalId found (fast path)
        var existingShow = CreateTvShow(id: 5, name: "Fast Path Show", tvdbId: 333);
        var externalId = new ShowExternalId
        {
            TvShowId = 5,
            TvShow = existingShow,
            Source = DataSource.SuperSubtitles,
            ExternalId = "1000"
        };

        _showExternalIdRepo
            .GetBySourceAndExternalIdAsync(DataSource.SuperSubtitles, "1000", Arg.Any<CancellationToken>())
            .Returns(externalId);

        // Act
        var result = await _sut.MergeShowAsync(
            DataSource.SuperSubtitles, "1000", "Fast Path Show",
            new ThirdPartyShowIds(333, null, null),
            CancellationToken.None);

        // Assert — fast path used, TvDB should not be queried
        result.Should().BeSameAs(existingShow);
        _tvShowRepo.DidNotReceive().GetByTvdbIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region MergeShowAsync — IMDB → TMDB enrichment

    [Test]
    public async Task MergeShowAsync_ImdbResolvesToTmdb_BackfillsDiscoveredTmdbId()
    {
        // Arrange — no direct match, but IMDB resolves to TMDB and matches
        _showExternalIdRepo
            .GetBySourceAndExternalIdAsync(DataSource.SuperSubtitles, "1100", Arg.Any<CancellationToken>())
            .ReturnsNull();

        _tmdbClient
            .FindByExternalIdAsync("tt0123456", "imdb_id", Arg.Any<CancellationToken>())
            .Returns(new FindByExternalIdResult
            {
                TvResults = [new ShowSearchResult { Id = 7777, Name = "Resolved Show" }],
                MovieResults = []
            });

        var existingShow = CreateTvShow(id: 60, name: "Resolved Show", tvdbId: null, tmdbId: 7777);
        _tvShowRepo
            .GetShowsByTmdbIdAsync(7777)
            .Returns(new[] { existingShow }.ToAsyncEnumerable());

        // Act
        var result = await _sut.MergeShowAsync(
            DataSource.SuperSubtitles, "1100", "Resolved Show",
            new ThirdPartyShowIds(null, "tt0123456", null),
            CancellationToken.None);

        // Assert — the resolved TMDB ID should be used for matching
        result.Should().BeSameAs(existingShow);
    }

    #endregion

    #region Helpers

    private static TvShow CreateTvShow(long id = 1, string name = "Test Show", int? tvdbId = null, int? tmdbId = null)
    {
        return new TvShow
        {
            Id = id,
            Name = name,
            TvdbId = tvdbId,
            TmdbId = tmdbId,
            Source = DataSource.Addic7ed,
            Discovered = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            Seasons = [],
            Episodes = [],
            ExternalIds = [],
            SeasonPackSubtitles = []
        };
    }

    private static Subtitle CreateSubtitle(DataSource source = DataSource.SuperSubtitles)
    {
        return new Subtitle
        {
            Source = source,
            ExternalId = "sub-ext-1",
            DownloadUri = new Uri($"https://feliratok.eu/download/{Guid.NewGuid()}"),
            Language = "English",
            Scene = "LOL",
            Version = 1,
            Completed = true,
            CompletionPct = 100.0,
            Discovered = DateTime.UtcNow
        };
    }

    #endregion
}
