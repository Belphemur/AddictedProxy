using AddictedProxy.Database.Context;
using AddictedProxy.OneTimeMigration.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Migrations.Services;

/// <summary>
/// One-time migration that backfills <see cref="Database.Model.Shows.Subtitle.Qualities" />
/// from the legacy <c>HD</c> boolean for existing Addic7ed subtitles.
/// <para>
/// Because Addic7ed only exposes a binary HD flag (no resolution detail), every subtitle
/// where <c>HD = true</c> is mapped to <c>Q720P | Q1080P</c> (bitmask value 12).
/// SuperSubtitles subtitles already receive exact quality flags from the import jobs and
/// are skipped here.
/// </para>
/// </summary>
[MigrationDate(2026, 2, 19)]
public class BackportHdToQualitiesMigration : IMigration
{
    // Q720P (4) | Q1080P (8) = 12
    private const int HdQualitiesMask = 12;
    private const int BatchSize = 5000;

    private readonly EntityContext _entityContext;
    private readonly ILogger<BackportHdToQualitiesMigration> _logger;

    public BackportHdToQualitiesMigration(EntityContext entityContext, ILogger<BackportHdToQualitiesMigration> logger)
    {
        _entityContext = entityContext;
        _logger = logger;
    }

    public async Task ExecuteAsync(Hangfire.Server.PerformContext context, CancellationToken token)
    {
        _logger.LogInformation(
            "Backporting HD=true → Qualities={Mask} (Q720P|Q1080P) for Addic7ed subtitles",
            HdQualitiesMask);

        int totalUpdated = 0;
        int updated;

        do
        {
            updated = await _entityContext.Database.ExecuteSqlRawAsync(
                $"""
                UPDATE "Subtitles"
                SET "Qualities" = {HdQualitiesMask}
                WHERE "Id" IN (
                    SELECT "Id" FROM "Subtitles"
                    WHERE "HD" = true
                      AND "Qualities" = 0
                    LIMIT {BatchSize}
                )
                """,
                token);

            totalUpdated += updated;
            _logger.LogInformation("Backported {Count} Addic7ed subtitle HD flags (running total: {Total})", updated, totalUpdated);
        }
        while (updated == BatchSize);

        _logger.LogInformation("BackportHdToQualitiesMigration complete: {Total} subtitles updated", totalUpdated);
    }
}
