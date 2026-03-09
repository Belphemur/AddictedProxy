using AddictedProxy.Database.Context;
using AddictedProxy.OneTimeMigration.Model;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Migrations.Services;

/// <summary>
/// One-time migration that creates missing Season entities for SeasonPackSubtitles
/// and then links them via the SeasonId FK.
/// This handles the case where season packs exist but the Season entity was never created.
/// </summary>
[MigrationDate(2026, 3, 9)]
public class CreateMissingSeasonsForSeasonPacksMigration : IMigration
{
    private readonly EntityContext _entityContext;
    private readonly ILogger<CreateMissingSeasonsForSeasonPacksMigration> _logger;

    public CreateMissingSeasonsForSeasonPacksMigration(
        EntityContext entityContext,
        ILogger<CreateMissingSeasonsForSeasonPacksMigration> logger)
    {
        _entityContext = entityContext;
        _logger = logger;
    }

    public async Task ExecuteAsync(PerformContext context, CancellationToken token)
    {
        context.WriteLine("Creating missing Season entities for SeasonPackSubtitles...");
        _logger.LogInformation("Creating missing Season entities for SeasonPackSubtitles...");

        // Step 1: Create missing Season entities
        // Insert into Seasons for any (TvShowId, Season number) combinations that exist in SeasonPackSubtitles
        // but don't have a corresponding Season entity
        var seasonsCreated = await _entityContext.Database.ExecuteSqlAsync(
            $"""
             INSERT INTO "Seasons" ("TvShowId", "Number", "LastRefreshed", "CreatedAt", "UpdatedAt")
             SELECT DISTINCT sp."TvShowId", sp."Season", NULL::timestamp with time zone, NOW(), NOW()
             FROM "SeasonPackSubtitles" sp
             WHERE NOT EXISTS (
                 SELECT 1 FROM "Seasons" s
                 WHERE s."TvShowId" = sp."TvShowId" AND s."Number" = sp."Season"
             )
             """, token);

        context.WriteLine($"Created {seasonsCreated} missing Season entities.");
        _logger.LogInformation("Created {SeasonsCreated} missing Season entities", seasonsCreated);

        // Step 2: Link SeasonPackSubtitles to their corresponding Season entities
        // Update all season packs that don't have a SeasonId yet
        var rowsAffected = await _entityContext.Database.ExecuteSqlAsync(
            $"""
             UPDATE "SeasonPackSubtitles" sp
             SET "SeasonId" = s."Id"
             FROM "Seasons" s
             WHERE sp."TvShowId" = s."TvShowId"
               AND sp."Season" = s."Number"
               AND sp."SeasonId" IS NULL
             """, token);

        context.WriteLine($"Linked {rowsAffected} SeasonPackSubtitle rows to their Season entities.");
        _logger.LogInformation("Linked {RowsAffected} SeasonPackSubtitle rows to their Season entities", rowsAffected);

        context.WriteLine("Migration completed successfully.");
        _logger.LogInformation("Migration completed successfully");
    }
}
