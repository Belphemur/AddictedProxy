using System.Collections.Concurrent;
using AddictedProxy.Caching.Extensions;
using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.OneTimeMigration.Model;
using AddictedProxy.Services.Provider.SeasonPack;
using AddictedProxy.Storage.Store;
using AddictedProxy.Tools.Database.Transaction;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Migrations.Services;

/// <summary>
/// One-time migration that re-catalogs every stored season pack from the ZIP blob in storage.
/// This replaces existing catalog rows so stale entries are removed when ZIP contents changed.
/// </summary>
[MigrationDate(2026, 3, 19)]
public class RecatalogStoredSeasonPacksMigration : IMigration
{
    private const int BatchSize = 100;
    private const int BatchConcurrency = 10;
    private const string ProgressCacheKey = "migration:season-pack-recatalog:next-batch";
    private static readonly DistributedCacheEntryOptions ProgressCacheEntryOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
    };

    private readonly EntityContext _entityContext;
    private readonly ITransactionManager<EntityContext> _transactionManager;
    private readonly IDistributedCache _distributedCache;
    private readonly ISeasonPackCatalogService _catalogService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RecatalogStoredSeasonPacksMigration> _logger;

    public RecatalogStoredSeasonPacksMigration(
        EntityContext entityContext,
        ITransactionManager<EntityContext> transactionManager,
        IDistributedCache distributedCache,
        ISeasonPackCatalogService catalogService,
        IServiceProvider serviceProvider,
        ILogger<RecatalogStoredSeasonPacksMigration> logger)
    {
        _entityContext = entityContext;
        _transactionManager = transactionManager;
        _distributedCache = distributedCache;
        _catalogService = catalogService;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task ExecuteAsync(PerformContext context, CancellationToken token)
    {
        context.WriteLine("Re-cataloging stored season packs from storage in batches...");

        var total = await _entityContext.SeasonPackSubtitles
            .AsNoTracking()
            .Where(pack => pack.StoragePath != null)
            .CountAsync(token);

        if (total == 0)
        {
            await _distributedCache.RemoveAsync(ProgressCacheKey, token);
            context.WriteLine("No stored season packs found to re-catalog.");
            _logger.LogInformation("No stored season packs found to re-catalog");
            return;
        }

        var progress = await _distributedCache.GetAsync<RecatalogStoredSeasonPacksProgress>(ProgressCacheKey, token);
        var nextBatchIndex = progress?.NextBatchIndex ?? 0;
        var totalBatches = (int)Math.Ceiling(total / (double)BatchSize);

        context.WriteLine($"Found {total} stored season packs to re-catalog across {totalBatches} batches.");
        if (nextBatchIndex > 0)
        {
            context.WriteLine($"Resuming from batch {nextBatchIndex + 1}.");
            _logger.LogInformation("Resuming season pack re-catalog from batch {BatchIndex}", nextBatchIndex + 1);
        }
        else
        {
            _logger.LogInformation("Starting re-catalog of {Count} stored season packs in {BatchCount} batches", total, totalBatches);
        }

        var progressBar = context.WriteProgressBar();
        var recataloged = 0;
        var failed = 0;
        progressBar.SetValue(Math.Min(nextBatchIndex * BatchSize, total) * 100.0 / total);

        while (true)
        {
            var batchRows = await _entityContext.SeasonPackSubtitles
                .AsNoTracking()
                .Where(pack => pack.StoragePath != null)
                .OrderBy(pack => pack.Id)
                .Skip(nextBatchIndex * BatchSize)
                .Take(BatchSize)
                .Select(pack => new { pack.Id, pack.UniqueId, pack.Filename, StoragePath = pack.StoragePath! })
                .ToListAsync(token);

            var batch = batchRows
                .Select((pack, index) => new SeasonPackBatchItem(index, pack.Id, pack.UniqueId, pack.Filename, pack.StoragePath))
                .ToArray();

            if (batch.Length == 0)
            {
                break;
            }

            context.WriteLine($"Processing batch {nextBatchIndex + 1}/{totalBatches} ({batch.Length} season packs)");
            var batchMessages = new ConcurrentQueue<string>();
            var batchRecataloged = 0;
            var batchFailed = 0;
            var downloads = new ConcurrentDictionary<int, SeasonPackDownloadResult>();

            await Parallel.ForEachAsync(batch, new ParallelOptions
            {
                CancellationToken = token,
                MaxDegreeOfParallelism = BatchConcurrency
            }, async (pack, cancellationToken) =>
            {
                try
                {
                    await using var scope = _serviceProvider.CreateAsyncScope();
                    var storageProvider = scope.ServiceProvider.GetRequiredService<IStorageProvider>();

                    await using var stream = await storageProvider.DownloadAsync(pack.StoragePath, cancellationToken);
                    if (stream == null)
                    {
                        downloads[pack.BatchOffset] = new SeasonPackDownloadResult(pack, null, $"WARNING: ZIP not found in storage for pack {pack.UniqueId} at {pack.StoragePath}");
                        return;
                    }

                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream, cancellationToken);
                    downloads[pack.BatchOffset] = new SeasonPackDownloadResult(pack, memoryStream.ToArray(), null);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    downloads[pack.BatchOffset] = new SeasonPackDownloadResult(pack, null, $"ERROR re-cataloging pack {pack.UniqueId}: {ex.Message}", ex);
                }
            });

            var orderedDownloads = downloads.Values
                .OrderBy(result => result.Pack.BatchOffset)
                .ToArray();

            await _transactionManager.WrapInTransactionAsync(async () =>
            {
                foreach (var download in orderedDownloads)
                {
                    if (download.ErrorMessage != null)
                    {
                        batchMessages.Enqueue(download.ErrorMessage);
                        if (download.Exception == null)
                        {
                            _logger.LogWarning("ZIP not found in storage for season pack {PackId} at {Path}", download.Pack.UniqueId, download.Pack.StoragePath);
                        }
                        else
                        {
                            _logger.LogError(download.Exception, "Failed to re-catalog season pack {PackId}", download.Pack.UniqueId);
                        }

                        batchFailed++;
                        continue;
                    }

                    await _catalogService.CatalogAndPersistAsync(new SeasonPackSubtitle
                    {
                        Id = download.Pack.Id,
                        Filename = download.Pack.Filename
                    }, download.ZipBlob!, token);

                    batchRecataloged++;
                }
            }, token);

            while (batchMessages.TryDequeue(out var message))
            {
                context.WriteLine(message);
            }

            recataloged += batchRecataloged;
            failed += batchFailed;
            nextBatchIndex++;

            await _distributedCache.SetAsync(
                ProgressCacheKey,
                new RecatalogStoredSeasonPacksProgress(nextBatchIndex),
                ProgressCacheEntryOptions,
                token);

            var processed = Math.Min(nextBatchIndex * BatchSize, total);
            progressBar.SetValue(processed * 100.0 / total);
            context.WriteLine($"Completed batch {nextBatchIndex}/{totalBatches}. Progress: {processed}/{total} season packs processed.");
            _logger.LogInformation(
                "Completed season pack re-catalog batch {BatchIndex}/{BatchCount}: {Processed}/{Total} processed, {Recataloged} re-cataloged, {Failed} failed",
                nextBatchIndex,
                totalBatches,
                processed,
                total,
                recataloged,
                failed);
        }

        await _distributedCache.RemoveAsync(ProgressCacheKey, token);

        context.WriteLine($"Re-catalog complete: {recataloged} re-cataloged, {failed} failed.");
        _logger.LogInformation(
            "Season pack re-catalog migration complete: {Recataloged} re-cataloged, {Failed} failed",
            recataloged,
            failed);
    }

    private sealed record SeasonPackBatchItem(int BatchOffset, long Id, Guid UniqueId, string Filename, string StoragePath);

    private sealed record SeasonPackDownloadResult(
        SeasonPackBatchItem Pack,
        byte[]? ZipBlob,
        string? ErrorMessage,
        Exception? Exception = null);

    private sealed record RecatalogStoredSeasonPacksProgress(int NextBatchIndex);
}