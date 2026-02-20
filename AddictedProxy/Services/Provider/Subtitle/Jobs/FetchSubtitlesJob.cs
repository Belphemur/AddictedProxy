#region

using AddictedProxy.Culture.Service;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Job.Filter;
using AddictedProxy.Services.Job.Model;
using AddictedProxy.Services.Provider.Episodes;
using AddictedProxy.Services.Provider.Seasons;
using AsyncKeyedLock;
using Hangfire;
using Locking;
using Performance.Model;
using Performance.Service;

#endregion

namespace AddictedProxy.Services.Provider.Subtitle.Jobs;

public class FetchSubtitlesJob
{
    private readonly ICultureParser _cultureParser;

    private readonly ILogger<FetchSubtitlesJob> _logger;
    private readonly ISeasonRefresher _seasonRefresher;
    private readonly IEpisodeRefresher _episodeRefresher;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly ITvShowRepository _tvShowRepository;
    private static readonly AsyncKeyedLocker<string> AsyncKeyedLocker = new(LockOptions.Default);
    private TvShow? _show;


    public FetchSubtitlesJob(ILogger<FetchSubtitlesJob> logger,
                             ICultureParser cultureParser,
                             ISeasonRefresher seasonRefresher,
                             IEpisodeRefresher episodeRefresher,
                             IPerformanceTracker performanceTracker,
                             ITvShowRepository tvShowRepository
    )
    {
        _logger = logger;
        _cultureParser = cultureParser;
        _seasonRefresher = seasonRefresher;
        _episodeRefresher = episodeRefresher;
        _performanceTracker = performanceTracker;
        _tvShowRepository = tvShowRepository;
    }


    private async Task<TvShow> GetShow(JobData data, CancellationToken token)
    {
        if (_show != null)
        {
            return _show;
        }

        return (_show = await _tvShowRepository.GetByIdAsync(data.ShowId, token)) ?? throw new InvalidOperationException($"Expected to find show {data.ShowId} in db.");
    }


    [UniqueJob(Order = 10)]
    [MaximumConcurrentExecutions(10, 10)]
    [AutomaticRetry(Attempts = 20, OnAttemptsExceeded = AttemptsExceededAction.Delete, DelaysInSeconds = [60, 10 * 60, 15 * 60, 45 * 60, 60 * 60, 10 * 60, 20 * 60, 40 * 60, 45 * 60, 60*60])]
    [Queue("fetch-subtitles")]
    public async Task ExecuteAsync(JobData data, CancellationToken cancellationToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromMinutes(10));
        var token = cts.Token;

        var show = await GetShow(data, token);
        using var scope = _logger.BeginScope(ScopeName(data, show));

        using var releaser = await AsyncKeyedLocker.LockOrNullAsync(data.Key, 0, token).ConfigureAwait(false);

        if (releaser is null)
        {
            _logger.LogInformation("Lock for {key} already taken", data.Key);
            return;
        }

        // Early-exit: check if a refresh is still needed before doing expensive work.
        // The same check is done at scheduling time in SearchSubtitlesService.TryScheduleJob,
        // but the state may have changed between scheduling and execution (e.g. another job already refreshed).
        var requestedSeason = show.Seasons.FirstOrDefault(s => s.Number == data.Season);
        if (requestedSeason == null && !_seasonRefresher.IsShowNeedsRefresh(show))
        {
            _logger.LogInformation("Show {show} doesn't need season refresh, skipping job", show.Name);
            return;
        }

        if (requestedSeason != null && !_episodeRefresher.IsSeasonNeedRefresh(show, requestedSeason))
        {
            _logger.LogInformation("Season S{season} of show {show} doesn't need episode refresh, skipping job", data.Season, show.Name);
            return;
        }

        using var transaction = _performanceTracker.BeginNestedSpan(nameof(FetchSubtitlesJob), "fetch-subtitles-one-episode");
        try
        {
            var season = await _seasonRefresher.GetRefreshSeasonAsync(show, data.Season, token);

            if (season == null)
            {
                _logger.LogInformation("Couldn't find season {season} for show {showName}", data.Season, show.Name);
                return;
            }

            var episode = await _episodeRefresher.GetRefreshEpisodeAsync(show, season, data.Episode, token);

            if (episode == null)
            {
                _logger.LogInformation("Couldn't find episode S{season}E{episode} for show {showName}", data.Season, data.Episode, show.Name);
                return;
            }

            var matchingSubtitles = await HasMatchingSubtitleAsync(data, episode, token);

            var latestDiscovered = episode.Subtitles.MaxBy(subtitle => subtitle.Discovered)?.Discovered ?? DateTime.UtcNow;

            if (matchingSubtitles || DateTime.UtcNow - latestDiscovered > TimeSpan.FromDays(180))
            {
                _logger.LogInformation("Matching subtitles found or episode was already refreshed");
            }
            else
            {
                _logger.LogInformation("Couldn't find matching subtitles for {search}", data.RequestData);
            }
        }
        catch (Exception e)
        {
            transaction.Finish(e, Status.InternalError);
            _logger.LogCritical(e, "Failed to fetch subtitles for {search}", data.RequestData);
            throw;
        }
    }

    private string ScopeName(JobData data, TvShow show)
    {
        return $"{show.Name} {data.RequestData}";
    }


    private async Task<bool> HasMatchingSubtitleAsync(JobData data, Episode episode, CancellationToken token)
    {
        var list = episode.Subtitles
                          .ToAsyncEnumerable()
                          .WhereAwait(async subtitle => data.Language == await _cultureParser.FromStringAsync(subtitle.Language, token));
        if (data.FileName != null)
        {
            list = list.Where(subtitle => subtitle.Scene.ToLowerInvariant().Split('+', '.', '-').Any(version => data.FileName.ToLowerInvariant().Contains(version)));
        }

        return await list.AnyAsync(token);
    }

    public readonly record struct JobData(long ShowId, int Season, int Episode, Culture.Model.Culture? Language, string? FileName) : IUniqueKey
    {
        public string Key => $"{ShowId}-{Season}";
        public string RequestData => $"S{Season}E{Episode} ({Language}) {FileName ?? ""}";
    }
}