using AddictedProxy.Database.Context;
using AddictedProxy.OneTimeMigration.Model;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Migrations.Services;

/// <summary>
/// Re-run of <see cref="CleanupEmptySeasonsMigration" />: polluted/empty seasons accumulated
/// again after the original migration already fired in production. Upstream SuperSubtitles
/// data could create empty seasons via season packs referencing non-existent seasons. The
/// ingestion guard now prevents new empty seasons from being created, and this migration
/// purges the existing ones (seasons with neither episodes nor season packs). Seasons that
/// still hold season packs are deliberately preserved as potential content.
/// </summary>
[MigrationDate(2026, 8, 8)]
public class CleanupEmptySeasonsMigrationAgain : IMigration
{
    private readonly EntityContext _entityContext;
    private readonly ILogger<CleanupEmptySeasonsMigrationAgain> _logger;

    public CleanupEmptySeasonsMigrationAgain(EntityContext entityContext, ILogger<CleanupEmptySeasonsMigrationAgain> logger)
    {
        _entityContext = entityContext;
        _logger = logger;
    }

    public async Task ExecuteAsync(PerformContext context, CancellationToken token)
    {
        context.WriteLine("Cleaning up empty seasons (no episodes, no season packs) across all shows (re-run)...");
        _logger.LogInformation("Cleaning up empty seasons across all shows (re-run)...");

        var deleted = await _entityContext.Database.ExecuteSqlAsync(
            $"""
             DELETE FROM "Seasons" s
             WHERE NOT EXISTS (
                 SELECT 1 FROM "Episodes" e
                 WHERE e."TvShowId" = s."TvShowId" AND e."Season" = s."Number"
             )
             AND NOT EXISTS (
                 SELECT 1 FROM "SeasonPackSubtitles" sp
                 WHERE sp."TvShowId" = s."TvShowId" AND sp."Season" = s."Number"
             )
             """, token);

        context.WriteLine($"Deleted {deleted} empty season(s) with no episodes and no season packs (re-run).");
        _logger.LogInformation("Deleted {Count} empty season(s) with no episodes and no season packs (re-run)", deleted);
    }
}
