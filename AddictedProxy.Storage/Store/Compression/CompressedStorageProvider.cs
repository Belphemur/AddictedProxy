using AddictedProxy.Storage.Store.Compression.Job;
using Compressor;
using Compressor.Factory;
using Hangfire;

namespace AddictedProxy.Storage.Store.Compression;

public class CompressedStorageProvider : ICompressedStorageProvider
{
    private readonly IStorageProvider _storageProvider;
    private readonly ICompressor _compressor;

    public CompressedStorageProvider(IStorageProvider storageProvider, ICompressor compressor)
    {
        _storageProvider = storageProvider;
        _compressor = compressor;
    }


    public async Task<bool> StoreAsync(string filename, Stream inputStream, CancellationToken cancellationToken)
    {
        await using var stream = await _compressor.CompressAsync(inputStream, cancellationToken);
        return await _storageProvider.StoreAsync(filename, stream, cancellationToken);
    }

    public async Task<Stream?> DownloadAsync(string filename, CancellationToken cancellationToken)
    {
        var stream = await _storageProvider.DownloadAsync(filename, cancellationToken);
        if (stream != null)
        {
            return await _compressor.DecompressAsync(stream, cancellationToken);
        }

        //old file name format using the extension of the compressor
        var oldPath = $"{filename}.brotli";
        stream = await _storageProvider.DownloadAsync(oldPath, cancellationToken);
        if (stream == null)
        {
            return stream;
        }

        BackgroundJob.Enqueue<MigrateCompressionJob>(x => x.ExecuteAsync(oldPath, null, cancellationToken));

        return await _compressor.DecompressAsync(AlgorithmEnum.BrotliDefault, stream, cancellationToken);
    }

    public Task DeleteAsync(string filename, CancellationToken cancellationToken)
    {
        return _storageProvider.DeleteAsync(filename, cancellationToken);
    }
}