using AddictedProxy.Storage.Extensions;
using ZstdSharp;

namespace AddictedProxy.Storage.Compressor.Factory.Impl;

public class ZstdCompressor : ICompressorService
{
    public CompressorType Enum => CompressorTypes.Zstd;

    public async Task<Stream> CompressAsync(Stream inputStream, CancellationToken cancellationToken)
    {
        var outputStream = new MemoryStream();
        {
            await using var compressionStream = new CompressionStream(outputStream, 3);
            await inputStream.CopyToAsync(compressionStream, cancellationToken);
            await compressionStream.FlushAsync(cancellationToken);
        }
        outputStream.ResetPosition();
        return outputStream;
    }

    public Task<Stream> DecompressAsync(Stream inputStream, CancellationToken cancellationToken)
    {
        var decompressionStream = new DecompressionStream(inputStream);
        return Task.FromResult<Stream>(decompressionStream);
    }
}