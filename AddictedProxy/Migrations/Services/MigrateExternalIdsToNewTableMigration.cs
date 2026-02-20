using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.OneTimeMigration.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Migrations.Services;

/// <summary>
/// One-time migration to populate the new ShowExternalId and EpisodeExternalId tables
/// from existing TvShow.ExternalId and Episode.ExternalId fields (Addic7ed source).
/// </summary>
[MigrationDate(2026, 2, 17)]
public class MigrateExternalIdsToNewTableMigration : IMigration
{
    private readonly EntityContext _entityContext;
    private readonly ILogger<MigrateExternalIdsToNewTableMigration> _logger;

    public MigrateExternalIdsToNewTableMigration(EntityContext entityContext, ILogger<MigrateExternalIdsToNewTableMigration> logger)
    {
        _entityContext = entityContext;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken token)
    {
        await MigrateShowExternalIdsAsync(token);
        await MigrateEpisodeExternalIdsAsync(token);
    }

    private async Task MigrateShowExternalIdsAsync(CancellationToken token)
    {
        _logger.LogInformation("Migrating TvShow.ExternalId to ShowExternalIds table");

        const int batchSize = 500;
        var totalMigrated = 0;

        // Get all shows that have an ExternalId and don't already have a ShowExternalId for Addic7ed
        var showIds = await _entityContext.TvShows
            .Where(show => !_entityContext.ShowExternalIds
                .Any(ext => ext.TvShowId == show.Id && ext.Source == DataSource.Addic7ed))
            .Select(show => new { show.Id, show.ExternalId })
            .ToListAsync(token);

        _logger.LogInformation("Found {Count} shows to migrate", showIds.Count);

        foreach (var batch in showIds.Chunk(batchSize))
        {
            var externalIds = batch.Select(show => new ShowExternalId
            {
                TvShowId = show.Id,
                Source = DataSource.Addic7ed,
                ExternalId = show.ExternalId.ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToArray();

            await _entityContext.ShowExternalIds.BulkInsertAsync(externalIds, token);
            totalMigrated += externalIds.Length;

            _logger.LogInformation("Migrated {Count}/{Total} show external IDs", totalMigrated, showIds.Count);
        }

        _logger.LogInformation("Completed migrating {Count} show external IDs", totalMigrated);
    }

    private async Task MigrateEpisodeExternalIdsAsync(CancellationToken token)
    {
        _logger.LogInformation("Migrating Episode.ExternalId to EpisodeExternalIds table");

        // Uses raw SQL because the Episode.ExternalId property has been removed from the entity.
        // This migration should already have been executed; it's kept for historical completeness.
        var migrated = await _entityContext.Database.ExecuteSqlAsync(
            $"""
             INSERT INTO "EpisodeExternalIds" ("EpisodeId", "Source", "ExternalId", "CreatedAt", "UpdatedAt")
             SELECT e."Id", {(int)DataSource.Addic7ed}, CAST(e."ExternalId" AS TEXT), NOW(), NOW()
             FROM "Episodes" e
             WHERE NOT EXISTS (
                 SELECT 1 FROM "EpisodeExternalIds" ext
                 WHERE ext."EpisodeId" = e."Id" AND ext."Source" = {(int)DataSource.Addic7ed}
             )
             """, token);

        _logger.LogInformation("Completed migrating {Count} episode external IDs", migrated);
    }
}
