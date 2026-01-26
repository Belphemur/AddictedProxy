using Compressor.Utils;
using ZstdSharp;

namespace Compressor.Factory.Impl;

public class ZstdCompressor : ICompressorService
{
    public AlgorithmEnum Enum => AlgorithmEnum.Zstd;


    public CompressorDefinition Definition { get; } = new("28-B5-2F-FD");

    public async Task<Stream> CompressAsync(Stream inputStream, CancellationToken cancellationToken)
    {
        var outputStream = new MemoryStream();
        {
            await using var compressionStream = new CompressionStream(outputStream, 5);
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