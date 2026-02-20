using AddictedProxy.Culture.Service;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Provider.Episodes;
using AddictedProxy.Services.Provider.Seasons;
using AddictedProxy.Services.Provider.Subtitle.Jobs;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Performance.Service;
using ISpan = Performance.Model.ISpan;

namespace AddictedProxy.Services.Tests.Provider.SubtitleJobs;

[TestFixture]
public class FetchSubtitlesJobTests
{
    private ILogger<FetchSubtitlesJob> _logger = null!;
    private ICultureParser _cultureParser = null!;
    private ISeasonRefresher _seasonRefresher = null!;
    private IEpisodeRefresher _episodeRefresher = null!;
    private IPerformanceTracker _performanceTracker = null!;
    private ITvShowRepository _tvShowRepository = null!;
    private FetchSubtitlesJob _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<FetchSubtitlesJob>>();
        _cultureParser = Substitute.For<ICultureParser>();
        _seasonRefresher = Substitute.For<ISeasonRefresher>();
        _episodeRefresher = Substitute.For<IEpisodeRefresher>();
        _performanceTracker = Substitute.For<IPerformanceTracker>();
        _tvShowRepository = Substitute.For<ITvShowRepository>();

        var mockSpan = Substitute.For<ISpan>();
        _performanceTracker.BeginNestedSpan(Arg.Any<string>(), Arg.Any<string>()).Returns(mockSpan);

        _sut = new FetchSubtitlesJob(
            _logger,
            _cultureParser,
            _seasonRefresher,
            _episodeRefresher,
            _performanceTracker,
            _tvShowRepository
        );
    }

    #region Early-exit: Season exists but doesn't need refresh

    [Test]
    public async Task ExecuteAsync_SeasonExistsAndNoRefreshNeeded_SkipsWithoutRefreshing()
    {
        // Arrange
        var show = CreateShowWithSeason(showId: 100, seasonNumber: 1, lastRefreshed: DateTime.UtcNow);
        _tvShowRepository.GetByIdAsync(100, Arg.Any<CancellationToken>()).Returns(show);
        _episodeRefresher.IsSeasonNeedRefresh(show, show.Seasons[0]).Returns(false);

        var jobData = new FetchSubtitlesJob.JobData(100, 1, 1, null, null);

        // Act
        await _sut.ExecuteAsync(jobData, CancellationToken.None);

        // Assert — should NOT have called GetRefreshSeasonAsync or GetRefreshEpisodeAsync
        await _seasonRefresher.DidNotReceive().GetRefreshSeasonAsync(Arg.Any<TvShow>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _episodeRefresher.DidNotReceive().GetRefreshEpisodeAsync(Arg.Any<TvShow>(), Arg.Any<Season>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Early-exit: Season not found and show doesn't need refresh

    [Test]
    public async Task ExecuteAsync_SeasonNotFoundAndShowNoRefreshNeeded_SkipsWithoutRefreshing()
    {
        // Arrange — show has season 1 but request is for season 2
        var show = CreateShowWithSeason(showId: 200, seasonNumber: 1, lastRefreshed: DateTime.UtcNow);
        _tvShowRepository.GetByIdAsync(200, Arg.Any<CancellationToken>()).Returns(show);
        _seasonRefresher.IsShowNeedsRefresh(show).Returns(false);

        var jobData = new FetchSubtitlesJob.JobData(200, 2, 1, null, null);

        // Act
        await _sut.ExecuteAsync(jobData, CancellationToken.None);

        // Assert — should NOT have called GetRefreshSeasonAsync or GetRefreshEpisodeAsync
        await _seasonRefresher.DidNotReceive().GetRefreshSeasonAsync(Arg.Any<TvShow>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _episodeRefresher.DidNotReceive().GetRefreshEpisodeAsync(Arg.Any<TvShow>(), Arg.Any<Season>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Proceed: Season exists and needs refresh

    [Test]
    public async Task ExecuteAsync_SeasonExistsAndNeedsRefresh_ProceedsWithRefresh()
    {
        // Arrange
        var show = CreateShowWithSeason(showId: 300, seasonNumber: 1, lastRefreshed: null);
        _tvShowRepository.GetByIdAsync(300, Arg.Any<CancellationToken>()).Returns(show);
        _episodeRefresher.IsSeasonNeedRefresh(show, show.Seasons[0]).Returns(true);

        var refreshedSeason = show.Seasons[0];
        _seasonRefresher.GetRefreshSeasonAsync(show, 1, Arg.Any<CancellationToken>()).Returns(refreshedSeason);

        var episode = CreateEpisode(1, 1);
        _episodeRefresher.GetRefreshEpisodeAsync(show, refreshedSeason, 1, Arg.Any<CancellationToken>()).Returns(episode);

        var jobData = new FetchSubtitlesJob.JobData(300, 1, 1, null, null);

        // Act
        await _sut.ExecuteAsync(jobData, CancellationToken.None);

        // Assert — should have called GetRefreshSeasonAsync (proceeded past early-exit)
        await _seasonRefresher.Received(1).GetRefreshSeasonAsync(show, 1, Arg.Any<CancellationToken>());
    }

    #endregion

    #region Proceed: Season not found and show needs refresh

    [Test]
    public async Task ExecuteAsync_SeasonNotFoundAndShowNeedsRefresh_ProceedsWithRefresh()
    {
        // Arrange — show has season 1 but request is for season 2
        var show = CreateShowWithSeason(showId: 400, seasonNumber: 1, lastRefreshed: DateTime.UtcNow);
        _tvShowRepository.GetByIdAsync(400, Arg.Any<CancellationToken>()).Returns(show);
        _seasonRefresher.IsShowNeedsRefresh(show).Returns(true);

        // Season refresh returns null (season 2 not found even after refresh)
        _seasonRefresher.GetRefreshSeasonAsync(show, 2, Arg.Any<CancellationToken>()).Returns((Season?)null);

        var jobData = new FetchSubtitlesJob.JobData(400, 2, 1, null, null);

        // Act
        await _sut.ExecuteAsync(jobData, CancellationToken.None);

        // Assert — should have called GetRefreshSeasonAsync (proceeded past early-exit)
        await _seasonRefresher.Received(1).GetRefreshSeasonAsync(show, 2, Arg.Any<CancellationToken>());
    }

    #endregion

    #region Helpers

    private static TvShow CreateShowWithSeason(long showId, int seasonNumber, DateTime? lastRefreshed)
    {
        var season = new Season
        {
            Id = showId * 10 + seasonNumber,
            TvShowId = showId,
            Number = seasonNumber,
            LastRefreshed = lastRefreshed
        };

        return new TvShow
        {
            Id = showId,
            Name = $"Test Show {showId}",
            Seasons = new List<Season> { season }
        };
    }

    private static Episode CreateEpisode(int seasonNumber, int episodeNumber)
    {
        return new Episode
        {
            Season = seasonNumber,
            Number = episodeNumber,
            Subtitles = new List<Database.Model.Shows.Subtitle>()
        };
    }

    #endregion
}
