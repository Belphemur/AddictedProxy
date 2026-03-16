using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.OneTimeMigration.Model;
using AddictedProxy.Services.Provider.SeasonPack;
using AddictedProxy.Storage.Store;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Migrations.Services;

/// <summary>
/// One-time migration that catalogs all existing stored season packs
/// that haven't been cataloged yet. Downloads each ZIP from S3 and
/// parses it into <see cref="SeasonPackEntry"/> records.
/// </summary>
[MigrationDate(2026, 3, 12)]
public class CatalogExistingSeasonPacksMigration : IMigration
{
    private readonly EntityContext _entityContext;
    private readonly IStorageProvider _storageProvider;
    private readonly ISeasonPackCatalogService _catalogService;
    private readonly ISeasonPackEntryRepository _entryRepository;
    private readonly ILogger<CatalogExistingSeasonPacksMigration> _logger;

    public CatalogExistingSeasonPacksMigration(
        EntityContext entityContext,
        IStorageProvider storageProvider,
        ISeasonPackCatalogService catalogService,
        ISeasonPackEntryRepository entryRepository,
        ILogger<CatalogExistingSeasonPacksMigration> logger)
    {
        _entityContext = entityContext;
        _storageProvider = storageProvider;
        _catalogService = catalogService;
        _entryRepository = entryRepository;
        _logger = logger;
    }

    public async Task ExecuteAsync(PerformContext context, CancellationToken token)
    {
        context.WriteLine("Cataloging existing stored season packs...");

        // Get all stored packs that haven't been cataloged yet
        var storedPacks = await _entityContext.SeasonPackSubtitles
            .Where(sp => sp.StoragePath != null)
            .ToListAsync(token);

        context.WriteLine($"Found {storedPacks.Count} stored season packs to check.");
        var progressBar = context.WriteProgressBar();
        var cataloged = 0;
        var skipped = 0;
        var failed = 0;

        for (var i = 0; i < storedPacks.Count; i++)
        {
            var pack = storedPacks[i];

            // Skip if already cataloged
            if (await _entryRepository.HasEntriesAsync(pack.Id, token))
            {
                skipped++;
                progressBar.SetValue((i + 1) * 100.0 / storedPacks.Count);
                continue;
            }

            try
            {
                await using var stream = await _storageProvider.DownloadAsync(pack.StoragePath!, token);
                if (stream == null)
                {
                    context.WriteLine($"WARNING: ZIP not found in storage for pack {pack.UniqueId} at {pack.StoragePath}");
                    _logger.LogWarning("ZIP not found in storage for season pack {PackId} at {Path}", pack.UniqueId, pack.StoragePath);
                    failed++;
                    progressBar.SetValue((i + 1) * 100.0 / storedPacks.Count);
                    continue;
                }

                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream, token);
                var blob = memoryStream.ToArray();

                await _catalogService.CatalogAndPersistAsync(pack, blob, token);
                cataloged++;
            }
            catch (Exception ex)
            {
                context.WriteLine($"ERROR cataloging pack {pack.UniqueId}: {ex.Message}");
                _logger.LogError(ex, "Failed to catalog season pack {PackId}", pack.UniqueId);
                failed++;
            }

            progressBar.SetValue((i + 1) * 100.0 / storedPacks.Count);
        }

        context.WriteLine($"Cataloging complete: {cataloged} cataloged, {skipped} already cataloged, {failed} failed.");
        _logger.LogInformation(
            "Season pack catalog migration complete: {Cataloged} cataloged, {Skipped} skipped, {Failed} failed",
            cataloged, skipped, failed);
    }
}
