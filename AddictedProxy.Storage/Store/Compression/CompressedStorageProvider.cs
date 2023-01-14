using AddictedProxy.Storage.Compressor;

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
        return await _storageProvider.StoreAsync(_compressor.GetFileName(filename), stream, cancellationToken);
    }

    public async Task<Stream?> DownloadAsync(string filename, CancellationToken cancellationToken)
    {
        var stream = await _storageProvider.DownloadAsync(_compressor.GetFileName(filename), cancellationToken);
        if (stream == null)
        {
            return stream;
        }

        return await _compressor.DecompressAsync(stream, cancellationToken);
    }
}