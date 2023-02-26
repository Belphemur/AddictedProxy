﻿#region

using AddictedProxy.Culture.Service;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Job.Filter;
using AddictedProxy.Services.Job.Model;
using AddictedProxy.Services.Provider.Episodes;
using AddictedProxy.Services.Provider.Seasons;
using Hangfire;
using Locking;
using Sentry.Performance.Model;
using Sentry.Performance.Service;

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
    [MaximumConcurrentExecutions(3, 10)]
    [AutomaticRetry(Attempts = 15, OnAttemptsExceeded = AttemptsExceededAction.Delete, DelaysInSeconds = new[] { 60, 10 * 60, 15 * 60, 45 * 60, 60 * 60, 10 * 60, 20 * 60, 40 * 60, 45 * 60 })]
    [Queue("fetch-subtitles")]
    public async Task ExecuteAsync(JobData data, CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(8));
        using var ctsLinked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);
        var token = ctsLinked.Token;

        var show = await GetShow(data, token);
        using var scope = _logger.BeginScope(ScopeName(data, show));
        var lockKey = Lock<FetchSubtitlesJob>.GetNamedKey(data.Key);
        if (Lock<FetchSubtitlesJob>.IsInUse(lockKey))
        {
            _logger.LogInformation("Lock for {key} already taken", data.Key);
            return;
        }

        using var _ = await Lock<FetchSubtitlesJob>.LockAsync(lockKey, token).ConfigureAwait(false);

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

            var latestDiscovered = episode.Subtitles.Max(subtitle => subtitle.Discovered);

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