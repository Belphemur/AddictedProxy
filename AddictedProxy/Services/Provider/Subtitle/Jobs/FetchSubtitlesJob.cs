#region

using System.Globalization;
using AddictedProxy.Culture.Service;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Job.Extensions;
using AddictedProxy.Services.Provider.Episodes;
using AddictedProxy.Services.Provider.Seasons;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Locking;
using NewRelic.Api.Agent;
using Polly.Contrib.WaitAndRetry;
using Sentry;
using Sentry.Performance.Model;
using Sentry.Performance.Service;

#endregion

namespace AddictedProxy.Services.Provider.Subtitle.Jobs;

public class FetchSubtitlesJob : IQueueJob
{
    private readonly CultureParser _cultureParser;

    private readonly ILogger<FetchSubtitlesJob> _logger;
    private readonly ISeasonRefresher _seasonRefresher;
    private readonly IEpisodeRefresher _episodeRefresher;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly ITvShowRepository _tvShowRepository;
    private TvShow? _show;


    public FetchSubtitlesJob(ILogger<FetchSubtitlesJob> logger,
                             CultureParser cultureParser,
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

    public JobData Data { get; set; }

    public IRetryAction FailRule { get; } = new ExponentialDecorrelatedJittedBackoffRetry(5, TimeSpan.FromMinutes(5));
    public TimeSpan? MaxRuntime { get; } = TimeSpan.FromMinutes(30);

    public string Key => Data.Key;
    public string QueueId => nameof(FetchSubtitlesJob);

    private async Task<TvShow> GetShow(CancellationToken token)
    {
        if (_show != null)
        {
            return _show;
        }

        return (_show = await _tvShowRepository.GetByIdAsync(Data.ShowId, token)) ?? throw new InvalidOperationException($"Expected to find show {Data.ShowId} in db.");
    }

    [Transaction(Web = false)]
    public async Task ExecuteAsync(CancellationToken token)
    {
        var show = await GetShow(token);
        using var scope = _logger.BeginScope(ScopeName(show));
        using var namedLock = Lock<FetchSubtitlesJob>.GetNamedLock(Data.Key);
        if (!await namedLock.WaitAsync(TimeSpan.Zero, token))
        {
            _logger.LogInformation("Lock for {key} already taken", Data.Key);
            return;
        }

        using var transaction = _performanceTracker.BeginNestedSpan(nameof(FetchSubtitlesJob), "fetch-subtitles-one-episode");
        try
        {
            var season = await _seasonRefresher.GetRefreshSeasonAsync(show, Data.Season, token);

            if (season == null)
            {
                _logger.LogInformation("Couldn't find season {season} for show {showName}", Data.Season, show.Name);
                return;
            }

            var episode = await _episodeRefresher.GetRefreshEpisodeAsync(show, season, Data.Episode, token);

            if (episode == null)
            {
                _logger.LogInformation("Couldn't find episode S{season}E{episode} for show {showName}", Data.Season, Data.Episode, show.Name);
                return;
            }

            var matchingSubtitles = await HasMatchingSubtitleAsync(episode, token);

            var latestDiscovered = episode.Subtitles.Max(subtitle => subtitle.Discovered);

            if (matchingSubtitles || DateTime.UtcNow - latestDiscovered > TimeSpan.FromDays(180))
            {
                _logger.LogInformation("Matching subtitles found or episode was already refreshed");
            }
            else
            {
                _logger.LogInformation("Couldn't find matching subtitles for {search}", Data.RequestData);
            }
        }
        catch (Exception)
        {
            transaction.Finish(Status.InternalError);
            throw;
        }
    }

    public async Task OnFailure(JobException exception)
    {
        var show = await GetShow(default);
        using var scope = _logger.BeginScope(ScopeName(show));
        _logger.LogJobException(exception, "Fetching subtitles info");
    }

    private string ScopeName(TvShow show)
    {
        return $"{show.Name} {Data.RequestData}";
    }


    private async Task<bool> HasMatchingSubtitleAsync(Episode episode, CancellationToken token)
    {
        var list = episode.Subtitles
                          .ToAsyncEnumerable()
                          .WhereAwait(async subtitle => Data.Language == await _cultureParser.FromStringAsync(subtitle.Language, token));
        if (Data.FileName != null)
        {
            list = list.Where(subtitle => subtitle.Scene.ToLowerInvariant().Split('+', '.', '-').Any(version => Data.FileName.ToLowerInvariant().Contains(version)));
        }

        return await list.AnyAsync(token);
    }

    public record JobData(long ShowId, int Season, int Episode, Culture.Model.Culture? Language, string? FileName)
    {
        public string Key => $"{ShowId}-{Season}";
        public string RequestData => $"S{Season}E{Episode} ({Language}) {FileName ?? ""}";
    }
}