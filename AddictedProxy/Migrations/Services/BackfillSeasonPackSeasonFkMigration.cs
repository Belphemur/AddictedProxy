using AddictedProxy.Database.Context;
using AddictedProxy.OneTimeMigration.Model;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Migrations.Services;

/// <summary>
/// One-time migration that backfills the SeasonId FK on SeasonPackSubtitles
/// by joining on TvShowId and Season number to the Seasons table.
/// </summary>
[MigrationDate(2026, 2, 24)]
public class BackfillSeasonPackSeasonFkMigration : IMigration
{
    private readonly EntityContext _entityContext;
    private readonly ILogger<BackfillSeasonPackSeasonFkMigration> _logger;

    public BackfillSeasonPackSeasonFkMigration(EntityContext entityContext, ILogger<BackfillSeasonPackSeasonFkMigration> logger)
    {
        _entityContext = entityContext;
        _logger = logger;
    }

    public async Task ExecuteAsync(PerformContext context, CancellationToken token)
    {
        context.WriteLine("Backfilling SeasonId FK on SeasonPackSubtitles...");
        _logger.LogInformation("Backfilling SeasonId FK on SeasonPackSubtitles...");

        var rowsAffected = await _entityContext.Database.ExecuteSqlAsync(
            $"""
             UPDATE "SeasonPackSubtitles" sp
             SET "SeasonId" = s."Id"
             FROM "Seasons" s
             WHERE sp."TvShowId" = s."TvShowId"
               AND sp."Season" = s."Number"
               AND sp."SeasonId" IS NULL
             """, token);

        context.WriteLine($"Backfilled {rowsAffected} SeasonPackSubtitle rows with SeasonId.");
        _logger.LogInformation("Backfilled {RowsAffected} SeasonPackSubtitle rows with SeasonId", rowsAffected);
    }
}
