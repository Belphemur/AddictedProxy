using AddictedProxy.Database.Context;
using AddictedProxy.OneTimeMigration.Model;
using AddictedProxy.Services.Provider.SeasonPack;
using AddictedProxy.Storage.Store;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Migrations.Services;

/// <summary>
/// One-time migration that re-catalogs every stored season pack from the ZIP blob in storage.
/// This replaces existing catalog rows so stale entries are removed when ZIP contents changed.
/// </summary>
[MigrationDate(2026, 3, 19)]
public class RecatalogStoredSeasonPacksMigration : IMigration
{
    private readonly EntityContext _entityContext;
    private readonly IStorageProvider _storageProvider;
    private readonly ISeasonPackCatalogService _catalogService;
    private readonly ILogger<RecatalogStoredSeasonPacksMigration> _logger;

    public RecatalogStoredSeasonPacksMigration(
        EntityContext entityContext,
        IStorageProvider storageProvider,
        ISeasonPackCatalogService catalogService,
        ILogger<RecatalogStoredSeasonPacksMigration> logger)
    {
        _entityContext = entityContext;
        _storageProvider = storageProvider;
        _catalogService = catalogService;
        _logger = logger;
    }

    public async Task ExecuteAsync(PerformContext context, CancellationToken token)
    {
        context.WriteLine("Re-cataloging stored season packs from storage...");

        var storedPacks = await _entityContext.SeasonPackSubtitles
            .Where(pack => pack.StoragePath != null)
            .ToListAsync(token);

        context.WriteLine($"Found {storedPacks.Count} stored season packs to re-catalog.");
        _logger.LogInformation("Starting re-catalog of {Count} stored season packs", storedPacks.Count);

        var progressBar = context.WriteProgressBar();
        var recataloged = 0;
        var failed = 0;

        for (var index = 0; index < storedPacks.Count; index++)
        {
            var pack = storedPacks[index];

            try
            {
                await using var stream = await _storageProvider.DownloadAsync(pack.StoragePath!, token);
                if (stream == null)
                {
                    context.WriteLine($"WARNING: ZIP not found in storage for pack {pack.UniqueId} at {pack.StoragePath}");
                    _logger.LogWarning("ZIP not found in storage for season pack {PackId} at {Path}", pack.UniqueId, pack.StoragePath);
                    failed++;
                    progressBar.SetValue((index + 1) * 100.0 / storedPacks.Count);
                    continue;
                }

                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream, token);
                await _catalogService.CatalogAndPersistAsync(pack, memoryStream.ToArray(), token);
                recataloged++;
            }
            catch (Exception ex)
            {
                context.WriteLine($"ERROR re-cataloging pack {pack.UniqueId}: {ex.Message}");
                _logger.LogError(ex, "Failed to re-catalog season pack {PackId}", pack.UniqueId);
                failed++;
            }

            progressBar.SetValue((index + 1) * 100.0 / storedPacks.Count);
        }

        context.WriteLine($"Re-catalog complete: {recataloged} re-cataloged, {failed} failed.");
        _logger.LogInformation(
            "Season pack re-catalog migration complete: {Recataloged} re-cataloged, {Failed} failed",
            recataloged,
            failed);
    }
}