#region

using System.Globalization;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Services.Culture;
using AddictedProxy.Services.Job.Extensions;
using AddictedProxy.Services.Provider.Episodes;
using AddictedProxy.Services.Provider.Seasons;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Locking;
using Sentry;
using Sentry.Performance.Model;
using Sentry.Performance.Service;

#endregion

namespace AddictedProxy.Services.Provider.Subtitle.Jobs;

public class FetchSubtitlesJob : IJob
{
    private readonly CultureParser _cultureParser;

    private readonly ILogger<FetchSubtitlesJob> _logger;
    private readonly ISeasonRefresher _seasonRefresher;
    private readonly IEpisodeRefresher _episodeRefresher;
    private readonly IPerformanceTracker _performanceTracker;


    public FetchSubtitlesJob(ILogger<FetchSubtitlesJob> logger,
        CultureParser cultureParser,
        ISeasonRefresher seasonRefresher,
        IEpisodeRefresher episodeRefresher,
        IPerformanceTracker performanceTracker
    )
    {
        _logger = logger;
        _cultureParser = cultureParser;
        _seasonRefresher = seasonRefresher;
        _episodeRefresher = episodeRefresher;
        _performanceTracker = performanceTracker;
    }

    public TimeSpan TimeBetweenChecks { get; } = TimeSpan.FromMinutes(30);
    public JobData Data { get; set; }


    public async Task ExecuteAsync(CancellationToken token)
    {
        using var scope = _logger.BeginScope(Data.ScopeName);
        using var namedLock = Lock<FetchSubtitlesJob>.GetNamedLock(Data.Key);
        if (!await namedLock.WaitAsync(TimeSpan.Zero, token))
        {
            _logger.LogInformation("Lock for {key} already taken", Data.Key);
            return;
        }

        using var transaction = _performanceTracker.BeginNestedSpan(nameof(FetchSubtitlesJob), $"Fetching subtitles for {Data.ScopeName}");

        var show = Data.Show;
        var season = await _seasonRefresher.GetRefreshSeasonAsync(show, Data.Season, token);

        if (season == null)
        {
            _logger.LogInformation("Couldn't find season {season} for show {showName}", Data.Season, Data.Show.Name);
            return;
        }

        var episode = await _episodeRefresher.GetRefreshEpisodeAsync(show, season, Data.Episode, token);

        if (episode == null)
        {
            _logger.LogInformation("Couldn't find episode S{season}E{episode} for show {showName}", Data.Season, Data.Episode, Data.Show.Name);
            return;
        }

        var matchingSubtitles = HasMatchingSubtitle(episode);

        var latestDiscovered = episode.Subtitles.Max(subtitle => subtitle.Discovered);

        if (matchingSubtitles || DateTime.UtcNow - latestDiscovered > TimeSpan.FromDays(180))
        {
            _logger.LogInformation("Matching subtitles found or episode was already refreshed");
        }
        else
        {
            _logger.LogInformation("Couldn't find matching subtitles for {search}", Data.ScopeName);
        }
    }

    public Task OnFailure(JobException exception)
    {
        using var scope = _logger.BeginScope(Data.ScopeName);
        _logger.LogJobException(exception, "Fetching subtitles info");
        return Task.CompletedTask;
    }

    public IRetryAction FailRule { get; } = new ExponentialBackoffRetry(TimeSpan.FromSeconds(5), 5);
    public TimeSpan? MaxRuntime { get; }


    private bool HasMatchingSubtitle(Episode episode)
    {
        var list = episode.Subtitles
            .Where(subtitle => Equals(_cultureParser.FromString(subtitle.Language), Data.Language));
        if (Data.FileName != null)
        {
            list = list.Where(subtitle => subtitle.Scene.ToLowerInvariant().Split('+', '.', '-').Any(version => Data.FileName.ToLowerInvariant().Contains(version)));
        }

        return list.Any();
    }

    public record JobData(TvShow Show, int Season, int Episode, CultureInfo? Language, string? FileName)
    {
        public string Key => $"{Show.Id}-{Season}";
        public string ScopeName => $"{Show.Name} S{Season}E{Episode} ({Language}) {FileName ?? ""}";
    }
}