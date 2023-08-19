#region

using System.IO.Compression;
using Compressor.Utils;

#endregion

namespace Compressor.Factory.Impl;

/// <summary>
/// No signature in the stream, default behavior of .NET currently (7.0) (2023-08-19)
/// </summary>
public class BrotliCompressor : ICompressorService
{
    public AlgorithmEnum Enum => AlgorithmEnum.BrotliDefault;

    public CompressorDefinition Definition { get; } = new("");

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