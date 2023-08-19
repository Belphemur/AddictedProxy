#region

using System.IO.Compression;
using AddictedProxy.Storage.Extensions;

#endregion

namespace AddictedProxy.Storage.Compressor.Factory.Impl;

public class BrotliCompressor : ICompressorService
{
    public CompressorType Enum => CompressorTypes.Brotli;

    /// <summary>
    /// Compress input stream to output stream
    /// </summary>
    /// <param name="inputStream">Input stream</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public async Task<Stream> CompressAsync(Stream inputStream, CancellationToken cancellationToken)
    {
        var outputStream = new MemoryStream();
        {
            await using var brotliStream = new BrotliStream(outputStream, CompressionLevel.Optimal, true);
            await inputStream.CopyToAsync(brotliStream, cancellationToken);
            await brotliStream.FlushAsync(cancellationToken);
        }
        outputStream.ResetPosition();
        return outputStream;
    }

    /// <summary>
    /// Decompress input stream to output stream
    /// </summary>
    /// <param name="inputStream">Input stream</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public Task<Stream> DecompressAsync(Stream inputStream, CancellationToken cancellationToken = default)
    {
        var brotliStream = new BrotliStream(inputStream, CompressionMode.Decompress);
        return Task.FromResult<Stream>(brotliStream);
    }
}