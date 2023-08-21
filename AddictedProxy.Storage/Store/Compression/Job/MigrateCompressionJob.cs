using Compressor;
using Compressor.Factory;
using Compressor.Utils;
using Hangfire;
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
    public async Task ExecuteAsync(string oldPath, CancellationToken token)
    {
        if (!oldPath.EndsWith(".brotli"))
        {
            _logger.LogError("Only for brotli files. Received {StoragePath}", oldPath);
            return;
        }

        //Remove .brotli
        var newPath = oldPath[..^7];

        var file = await _storageProvider.DownloadAsync(oldPath, token);
        if (file == null)
        {
            _logger.LogError("Couldn't find the file {StoragePath} to migrate", oldPath);
            return;
        }

        await using var compressedFile = await _compressor.DecompressAsync(AlgorithmEnum.BrotliDefault, file, token);
        using var memoryStream = new MemoryStream();
        await compressedFile.CopyToAsync(memoryStream, token);
        memoryStream.ResetPosition();

        var compressed = await _compressor.CompressAsync(memoryStream, token);

        await _storageProvider.StoreAsync(newPath, compressed, token);
        await _storageProvider.DeleteAsync(oldPath, token);
    }
}