using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.OneTimeMigration.Model;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Migrations.Services;

/// <summary>
/// One-time migration that normalizes existing Addic7ed subtitle Scene values: the Addic7ed
/// version column joins multiple release groups with '+' (e.g. "NTb+playWEB+Kitsune"), while
/// the parser now stores them comma-separated like the SuperSubtitles source. Scene is ignored
/// on merge updates, so existing rows would never be refreshed by the regular ingestion pipeline.
/// </summary>
[MigrationDate(2026, 8, 10)]
public class NormalizeSubtitleSceneSeparatorMigration : IMigration
{
    private const int Addic7edSource = (int)DataSource.Addic7ed;

    private readonly EntityContext _entityContext;
    private readonly ILogger<NormalizeSubtitleSceneSeparatorMigration> _logger;

    public NormalizeSubtitleSceneSeparatorMigration(EntityContext entityContext, ILogger<NormalizeSubtitleSceneSeparatorMigration> logger)
    {
        _entityContext = entityContext;
        _logger = logger;
    }

    public async Task ExecuteAsync(PerformContext context, CancellationToken token)
    {
        context.WriteLine("Normalizing '+' separators in Addic7ed subtitle Scene values...");
        _logger.LogInformation("Normalizing '+' separators in Addic7ed subtitle Scene values...");

        var updated = await _entityContext.Database.ExecuteSqlAsync(
            $"""
             UPDATE "Subtitles"
             SET "Scene" = regexp_replace("Scene", '\s*\+\s*', ', ', 'g')
             WHERE "Source" = {Addic7edSource} AND "Scene" LIKE '%+%'
             """, token);

        context.WriteLine($"Normalized {updated} Addic7ed subtitle Scene value(s).");
        _logger.LogInformation("Normalized {Count} Addic7ed subtitle Scene value(s) containing '+'", updated);
    }
}
