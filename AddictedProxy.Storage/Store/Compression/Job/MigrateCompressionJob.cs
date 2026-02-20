using Compressor;
using Compressor.Factory;
using Compressor.Utils;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Storage.Store.Compression.Job;

public class MigrateCompressionJob
{
    private readonly ICompressor _compressor;
    private readonly IStorageProvider _storageProvider;
    private readonly ILogger<MigrateCompressionJob> _logger;

    public MigrateCompressionJob(ICompressor compressor, IStorageProvider storageProvider, ILogger<MigrateCompressionJob> logger)
    {
        _compressor = compressor;
        _storageProvider = storageProvider;
        _logger = logger;
    }

    [Queue("store-subtitle")]
    public async Task ExecuteAsync(string oldPath, PerformContext context, CancellationToken token)
    {
        context.WriteLine(string.Format("Starting compression migration for {0}", oldPath));
        if (!oldPath.EndsWith(".brotli"))
        {
            _logger.LogError("Only for brotli files. Received {StoragePath}", oldPath);
            context.WriteLine(string.Format("Error: File is not Brotli compressed: {0}", oldPath));
            return;
        }

        //Remove .brotli
        var newPath = oldPath.Substring(0, oldPath.Length - 7);
        context.WriteLine(string.Format("Will migrate to: {0}", newPath));

        var file = await _storageProvider.DownloadAsync(oldPath, token);
        if (file == null)
        {
            _logger.LogError("Couldn't find the file {StoragePath} to migrate", oldPath);
            context.WriteLine(string.Format("Error: Could not download file from storage: {0}", oldPath));
            return;
        }

        context.WriteLine("Decompressing Brotli file...");
        await using var compressedFile = await _compressor.DecompressAsync(AlgorithmEnum.BrotliDefault, file, token);
        using var memoryStream = new MemoryStream();
        await compressedFile.CopyToAsync(memoryStream, token);
        memoryStream.ResetPosition();

        context.WriteLine("Compressing with Zstandard...");
        var compressed = await _compressor.CompressAsync(memoryStream, token);

        context.WriteLine("Storing compressed file...");
        await _storageProvider.StoreAsync(newPath, compressed, token);
        context.WriteLine("Deleting old Brotli file...");
        await _storageProvider.DeleteAsync(oldPath, token);
        context.WriteLine(string.Format("Successfully migrated compression for {0}", oldPath));
    }
}