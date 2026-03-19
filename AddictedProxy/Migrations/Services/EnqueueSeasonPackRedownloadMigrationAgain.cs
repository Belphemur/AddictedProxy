using AddictedProxy.Database.Context;
using AddictedProxy.OneTimeMigration.Model;
using AddictedProxy.Services.Provider.SeasonPack;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Migrations.Services;

/// <summary>
/// One-time migration that enqueues a forced re-download for every season pack.
/// Each enqueued job re-fetches from upstream and stores using the v2 S3 prefix.
/// </summary>
[MigrationDate(2026, 3, 19)]
public class EnqueueSeasonPackRedownloadMigrationAgain : IMigration
{
    private readonly EntityContext _entityContext;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly ILogger<EnqueueSeasonPackRedownloadMigrationAgain> _logger;

    public EnqueueSeasonPackRedownloadMigrationAgain(
        EntityContext entityContext,
        IBackgroundJobClient backgroundJobClient,
        ILogger<EnqueueSeasonPackRedownloadMigrationAgain> logger)
    {
        _entityContext = entityContext;
        _backgroundJobClient = backgroundJobClient;
        _logger = logger;
    }

    public async Task ExecuteAsync(PerformContext context, CancellationToken token)
    {
        var seasonPackIds = await _entityContext.SeasonPackSubtitles
            .AsNoTracking()
            .Select(pack => pack.UniqueId)
            .ToListAsync(token);

        context.WriteLine($"Found {seasonPackIds.Count} season packs to re-download");
        _logger.LogInformation("Enqueuing forced re-download for {Count} season packs", seasonPackIds.Count);

        foreach (var seasonPackId in seasonPackIds)
        {
            _backgroundJobClient.Enqueue<StoreSeasonPackJob>(job =>
                job.DownloadAndStoreAsync(new StoreSeasonPackJob.JobData(seasonPackId, ForceRedownload: true), null!, default));
        }

        context.WriteLine($"Enqueued {seasonPackIds.Count} StoreSeasonPackJob jobs");
    }
}
