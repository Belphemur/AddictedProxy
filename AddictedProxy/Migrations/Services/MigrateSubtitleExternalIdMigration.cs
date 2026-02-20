using AddictedProxy.Database.Context;
using AddictedProxy.OneTimeMigration.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Migrations.Services;

/// <summary>
/// One-time migration to populate the new <see cref="Database.Model.Shows.Subtitle.ExternalId"/> field
/// from the existing <see cref="Database.Model.Shows.Subtitle.DownloadUri"/> for Addic7ed subtitles.
/// </summary>
[MigrationDate(2026, 2, 17)]
public class MigrateSubtitleExternalIdMigration : IMigration
{
    private readonly EntityContext _entityContext;
    private readonly ILogger<MigrateSubtitleExternalIdMigration> _logger;

    public MigrateSubtitleExternalIdMigration(EntityContext entityContext, ILogger<MigrateSubtitleExternalIdMigration> logger)
    {
        _entityContext = entityContext;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken token)
    {
        _logger.LogInformation("Populating Subtitle.ExternalId from DownloadUri for existing subtitles");

        const int batchSize = 5000;
        int totalUpdated;

        // Use raw SQL for efficiency — directly set ExternalId = DownloadUri::text for all rows where ExternalId is null
        do
        {
            totalUpdated = await _entityContext.Database.ExecuteSqlRawAsync(
                """
                UPDATE "Subtitles"
                SET "ExternalId" = "DownloadUri"
                WHERE "Id" IN (
                    SELECT "Id" FROM "Subtitles"
                    WHERE "ExternalId" IS NULL
                    LIMIT {0}
                )
                """,
                [batchSize],
                token);

            _logger.LogInformation("Updated {Count} subtitle ExternalId values", totalUpdated);
        } while (totalUpdated == batchSize);

        _logger.LogInformation("Completed populating Subtitle.ExternalId from DownloadUri");
    }
}
