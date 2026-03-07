using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Job.Filter;
using AddictedProxy.Services.Job.Model;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Performance.Service;

namespace AddictedProxy.Services.Provider.Shows.Jobs;

/// <summary>
/// Hangfire job that deletes seasons which have neither episodes nor season packs for a given show.
/// Intended to run as a continuation after fetch-subtitles or <see cref="RefreshSingleShowJob"/>.
/// </summary>
public class CleanupEmptySeasonsJob
{
    private readonly ISeasonRepository _seasonRepository;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly ILogger<CleanupEmptySeasonsJob> _logger;

    public CleanupEmptySeasonsJob(ISeasonRepository seasonRepository,
                                  IPerformanceTracker performanceTracker,
                                  ILogger<CleanupEmptySeasonsJob> logger)
    {
        _seasonRepository = seasonRepository;
        _performanceTracker = performanceTracker;
        _logger = logger;
    }

    /// <summary>
    /// Remove empty seasons (no episodes, no season packs) for the specified show.
    /// </summary>
    [UniqueJob]
    [Queue("default")]
    public async Task ExecuteAsync(JobData data, PerformContext context, CancellationToken cancellationToken)
    {
        context.WriteLine($"Cleaning up empty seasons for show {data.ShowId}");
        using var span = _performanceTracker.BeginNestedSpan("cleanup-empty-seasons", $"show-{data.ShowId}");

        var deleted = await _seasonRepository.DeleteEmptySeasonsForShowAsync(data.ShowId, cancellationToken);

        _logger.LogInformation("Deleted {Count} empty season(s) for show {ShowId}", deleted, data.ShowId);
        context.WriteLine($"Deleted {deleted} empty season(s) for show {data.ShowId}");
    }

    /// <summary>Job data carrying the show to clean up.</summary>
    public readonly record struct JobData(long ShowId) : IUniqueKey
    {
        public string Key => $"cleanup-empty-seasons:{ShowId}";
    }
}
