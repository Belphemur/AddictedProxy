using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.OneTimeMigration.Model;
using AddictedProxy.Services.Provider.SuperSubtitles.Config;
using AddictedProxy.Services.Provider.SuperSubtitles.Jobs;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AddictedProxy.Migrations.Services;

/// <summary>
/// One-time migration that cleans up all data previously imported from SuperSubtitles.
/// Truncates season pack subtitles, subtitles, episode external IDs, show external IDs,
/// SuperSubtitles state, and removes episodes that only have subtitles from SuperSubtitles.
/// After cleanup, schedules a fresh bulk import job.
/// </summary>
[MigrationDate(2026, 2, 21)]
public class CleanSuperSubtitlesDataMigration : IMigration
{
    private const int SuperSubtitlesSource = (int)DataSource.SuperSubtitles;

    private readonly EntityContext _entityContext;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly SuperSubtitlesImportConfig _importConfig;
    private readonly ILogger<CleanSuperSubtitlesDataMigration> _logger;

    public CleanSuperSubtitlesDataMigration(
        EntityContext entityContext,
        IBackgroundJobClient backgroundJobClient,
        IOptions<SuperSubtitlesImportConfig> importConfig,
        ILogger<CleanSuperSubtitlesDataMigration> logger)
    {
        _entityContext = entityContext;
        _backgroundJobClient = backgroundJobClient;
        _importConfig = importConfig.Value;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken token)
    {
        _logger.LogInformation("Starting SuperSubtitles data cleanup...");

        // 1. Delete SuperSubtitles season pack subtitles
        var seasonPacksDeleted = await _entityContext.Database.ExecuteSqlAsync(
            $"""DELETE FROM "SeasonPackSubtitles" WHERE "Source" = {SuperSubtitlesSource}""", token);
        _logger.LogInformation("Deleted {Count} SuperSubtitles season pack subtitles", seasonPacksDeleted);

        // 2. Delete SuperSubtitles subtitles
        var subtitlesDeleted = await _entityContext.Database.ExecuteSqlAsync(
            $"""DELETE FROM "Subtitles" WHERE "Source" = {SuperSubtitlesSource}""", token);
        _logger.LogInformation("Deleted {Count} SuperSubtitles subtitles", subtitlesDeleted);

        // 3. Delete episodes that only had subtitles from SuperSubtitles (no remaining subtitles)
        var episodesDeleted = await _entityContext.Database.ExecuteSqlAsync(
            $"""
             DELETE FROM "Episodes" e
             WHERE NOT EXISTS (
                 SELECT 1 FROM "Subtitles" s WHERE s."EpisodeId" = e."Id"
             )
             AND EXISTS (
                 SELECT 1 FROM "EpisodeExternalIds" ext
                 WHERE ext."EpisodeId" = e."Id" AND ext."Source" = {SuperSubtitlesSource}
             )
             """, token);
        _logger.LogInformation("Deleted {Count} episodes that only had SuperSubtitles subtitles", episodesDeleted);

        // 4. Delete SuperSubtitles episode external IDs
        var episodeExtIdsDeleted = await _entityContext.Database.ExecuteSqlAsync(
            $"""DELETE FROM "EpisodeExternalIds" WHERE "Source" = {SuperSubtitlesSource}""", token);
        _logger.LogInformation("Deleted {Count} SuperSubtitles episode external IDs", episodeExtIdsDeleted);

        // 5. Delete SuperSubtitles show external IDs
        var showExtIdsDeleted = await _entityContext.Database.ExecuteSqlAsync(
            $"""DELETE FROM "ShowExternalIds" WHERE "Source" = {SuperSubtitlesSource}""", token);
        _logger.LogInformation("Deleted {Count} SuperSubtitles show external IDs", showExtIdsDeleted);

        // 6. Truncate SuperSubtitles state (sync cursor)
        var stateDeleted = await _entityContext.Database.ExecuteSqlAsync(
            $"""DELETE FROM "SuperSubtitlesState" """, token);
        _logger.LogInformation("Cleared SuperSubtitles state ({Count} rows)", stateDeleted);

        _logger.LogInformation("SuperSubtitles data cleanup complete. Scheduling fresh import...");

        // 7. Schedule a fresh bulk import if imports are enabled
        if (_importConfig.EnableImport)
        {
            _backgroundJobClient.Enqueue<ImportSuperSubtitlesJob>(job => job.ExecuteAsync(null!, CancellationToken.None));
            _logger.LogInformation("Scheduled fresh SuperSubtitles bulk import job");
        }
        else
        {
            _logger.LogInformation("SuperSubtitles import is disabled — skipping import scheduling");
        }
    }
}
