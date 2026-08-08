using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Job.Filter;
using AddictedProxy.Services.Job.Model;
using AsyncKeyedLock;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Locking;
using Performance.Service;

namespace AddictedProxy.Services.Provider.Shows.Jobs;

/// <summary>
/// Hangfire job that deletes a show's seasons whose number exceeds the real season count
/// known from TMDB (<see cref="Database.Model.Shows.TvShow.NumberOfSeasons"/>),
/// including their episodes, subtitles and season packs.
/// Intended to prune bogus seasons created by polluted provider streams.
/// </summary>
public class PruneInvalidSeasonsJob
{
    private readonly ISeasonRepository _seasonRepository;
    private readonly ITvShowRepository _tvShowRepository;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly ILogger<PruneInvalidSeasonsJob> _logger;
    // Serializes prune runs for the same show so that concurrent prune executions
    // cannot interleave their cascading deletes.
    private static readonly AsyncKeyedLocker<long> ShowLocker = new(LockOptions.Default);

    public PruneInvalidSeasonsJob(ISeasonRepository seasonRepository,
                                  ITvShowRepository tvShowRepository,
                                  IPerformanceTracker performanceTracker,
                                  ILogger<PruneInvalidSeasonsJob> logger)
    {
        _seasonRepository = seasonRepository;
        _tvShowRepository = tvShowRepository;
        _performanceTracker = performanceTracker;
        _logger = logger;
    }

    /// <summary>
    /// Remove seasons (and their episodes, subtitles and season packs) whose number
    /// exceeds the show's known TMDB season count.
    /// </summary>
    [UniqueJob]
    [Queue("default")]
    public async Task ExecuteAsync(JobData data, PerformContext context, CancellationToken cancellationToken)
    {
        context.WriteLine($"Pruning invalid seasons for show {data.ShowId}");
        using var span = _performanceTracker.BeginNestedSpan("prune-invalid-seasons", $"show-{data.ShowId}");

        var show = await _tvShowRepository.GetByIdAsync(data.ShowId, cancellationToken);
        if (show == null)
        {
            _logger.LogWarning("Could not find show {ShowId} to prune invalid seasons", data.ShowId);
            context.WriteLine($"Show {data.ShowId} not found, skipping");
            return;
        }

        if (show.NumberOfSeasons == null)
        {
            context.WriteLine($"Show {data.ShowId} has no known TMDB season count, skipping");
            return;
        }

        using var releaser = await ShowLocker.LockAsync(data.ShowId, cancellationToken);

        var deleted = await _seasonRepository.DeleteSeasonsBeyondAsync(data.ShowId, show.NumberOfSeasons.Value, cancellationToken);

        if (deleted > 0)
        {
            _logger.LogInformation("Pruned {Count} invalid season(s) beyond season {MaxSeason} for show {ShowName} ({ShowId})",
                deleted, show.NumberOfSeasons.Value, show.Name, data.ShowId);
        }

        context.WriteLine($"Pruned {deleted} invalid season(s) beyond season {show.NumberOfSeasons.Value} for show {data.ShowId}");
    }

    /// <summary>Job data carrying the show to prune.</summary>
    public readonly record struct JobData(long ShowId) : IUniqueKey
    {
        public string Key => $"prune-invalid-seasons:{ShowId}";
    }
}
