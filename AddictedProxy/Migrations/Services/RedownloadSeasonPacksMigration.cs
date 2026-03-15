using AddictedProxy.Database.Context;
using AddictedProxy.OneTimeMigration.Model;
using AddictedProxy.Services.Provider.SeasonPack;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Migrations.Services;

/// <summary>
/// One-time migration that clears all stored season packs and enqueues
/// background jobs to re-download and re-store them from upstream.
/// This fixes potential ZIP corruption in existing stored season packs.
/// </summary>
[MigrationDate(2026, 3, 15)]
public class RedownloadSeasonPacksMigration : IMigration
{
    private readonly EntityContext _entityContext;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly ILogger<RedownloadSeasonPacksMigration> _logger;

    public RedownloadSeasonPacksMigration(
        EntityContext entityContext,
        IBackgroundJobClient backgroundJobClient,
        ILogger<RedownloadSeasonPacksMigration> logger)
    {
        _entityContext = entityContext;
        _backgroundJobClient = backgroundJobClient;
        _logger = logger;
    }

    public async Task ExecuteAsync(PerformContext context, CancellationToken token)
    {
        context.WriteLine("Clearing stored season packs for re-download...");

        // Find all season packs that have been stored (potentially corrupt)
        var storedPacks = await _entityContext.SeasonPackSubtitles
            .Where(sp => sp.StoragePath != null)
            .ToListAsync(token);

        context.WriteLine($"Found {storedPacks.Count} stored season packs to re-download.");

        if (storedPacks.Count == 0)
        {
            context.WriteLine("No stored season packs found, nothing to do.");
            return;
        }

        // Clear StoragePath/StoredAt so they can be re-stored by the background job
        foreach (var pack in storedPacks)
        {
            pack.StoragePath = null;
            pack.StoredAt = null;
        }

        await _entityContext.SaveChangesAsync(token);
        context.WriteLine($"Cleared storage paths for {storedPacks.Count} season packs.");

        // Enqueue a background job for each season pack to re-download and re-store
        var enqueued = 0;
        foreach (var pack in storedPacks)
        {
            _backgroundJobClient.Enqueue<StoreSeasonPackJob>(job => job.DownloadAndStoreAsync(pack.UniqueId, null!, default));
            enqueued++;
        }

        context.WriteLine($"Enqueued {enqueued} season packs for re-download.");
        _logger.LogInformation(
            "Season pack re-download migration complete: cleared {Cleared} storage paths, enqueued {Enqueued} re-download jobs",
            storedPacks.Count, enqueued);
    }
}
