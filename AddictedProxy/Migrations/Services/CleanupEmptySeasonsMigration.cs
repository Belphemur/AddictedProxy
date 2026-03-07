using AddictedProxy.Database.Context;
using AddictedProxy.OneTimeMigration.Model;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Migrations.Services;

/// <summary>
/// One-time migration that deletes all seasons across all shows that have neither
/// episodes nor season pack subtitles. These empty seasons were leaking into the
/// database and surfacing in the UI. They will be recreated on the next refresh
/// if they actually exist on the upstream provider.
/// </summary>
[MigrationDate(2026, 3, 7)]
public class CleanupEmptySeasonsMigration : IMigration
{
    private readonly EntityContext _entityContext;
    private readonly ILogger<CleanupEmptySeasonsMigration> _logger;

    public CleanupEmptySeasonsMigration(EntityContext entityContext, ILogger<CleanupEmptySeasonsMigration> logger)
    {
        _entityContext = entityContext;
        _logger = logger;
    }

    public async Task ExecuteAsync(PerformContext context, CancellationToken token)
    {
        context.WriteLine("Cleaning up empty seasons (no episodes, no season packs) across all shows...");
        _logger.LogInformation("Cleaning up empty seasons across all shows...");

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

        context.WriteLine($"Deleted {deleted} empty season(s) with no episodes and no season packs.");
        _logger.LogInformation("Deleted {Count} empty season(s) with no episodes and no season packs", deleted);
    }
}
