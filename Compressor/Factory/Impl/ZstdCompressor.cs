using Compressor.Utils;
using ZstdSharp;

namespace Compressor.Factory.Impl;

public class ZstdCompressor : ICompressorService, IDisposable
{
    public CompressorType Enum => CompressorTypes.Zstd;

    private readonly ZstdSharp.Compressor _compressor = new(3);
    private readonly Decompressor _decompressor = new();

    public async Task<Stream> CompressAsync(Stream inputStream, CancellationToken cancellationToken)
    {
        var outputStream = new MemoryStream();
        {
            await using var compressionStream = new CompressionStream(outputStream, _compressor);
            await inputStream.CopyToAsync(compressionStream, cancellationToken);
            await compressionStream.FlushAsync(cancellationToken);
        }
        outputStream.ResetPosition();
        return outputStream;
    }

    public Task<Stream> DecompressAsync(Stream inputStream, CancellationToken cancellationToken)
    {
        var decompressionStream = new DecompressionStream(inputStream, _decompressor);
        return Task.FromResult<Stream>(decompressionStream);
    }

    public void Dispose()
    {
        _compressor.Dispose();
        _decompressor.Dispose();
    }
}