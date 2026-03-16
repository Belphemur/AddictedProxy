using AddictedProxy.Database.Context;
using AddictedProxy.OneTimeMigration.Model;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Migrations.Services;

/// <summary>
/// One-time migration that clears season pack storage metadata so existing
/// ZIP blobs can be removed from S3 and re-stored under the v2 key prefix.
/// </summary>
[MigrationDate(2026, 3, 14)]
public class ResetSeasonPackStorageForV2PrefixMigration : IMigration
{
    private readonly EntityContext _entityContext;
    private readonly ILogger<ResetSeasonPackStorageForV2PrefixMigration> _logger;

    public ResetSeasonPackStorageForV2PrefixMigration(
        EntityContext entityContext,
        ILogger<ResetSeasonPackStorageForV2PrefixMigration> logger)
    {
        _entityContext = entityContext;
        _logger = logger;
    }

    public async Task ExecuteAsync(PerformContext context, CancellationToken token)
    {
        context.WriteLine("Resetting season pack storage metadata for v2 prefix migration...");
        _logger.LogInformation("Resetting season pack storage metadata for v2 prefix migration...");

        var rowsAffected = await _entityContext.Database.ExecuteSqlAsync(
            $"""
             UPDATE "SeasonPackSubtitles"
             SET "StoragePath" = NULL,
                 "StoredAt" = NULL
             WHERE "StoragePath" IS NOT NULL
             """, token);

        context.WriteLine($"Cleared storage metadata for {rowsAffected} season packs.");
        _logger.LogInformation("Cleared storage metadata for {RowsAffected} season packs", rowsAffected);
    }
}
