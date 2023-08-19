#region

using System.IO.Compression;
using Compressor.Utils;

#endregion

namespace Compressor.Factory.Impl;

/// <summary>
/// Has the magic number written into the stream
/// </summary>
public class BrotliSignedCompressor : ICompressorService
{
    public AlgorithmEnum Enum => AlgorithmEnum.BrotliWithSignature;

    public CompressorDefinition Definition { get; } =  new("CE-B2-CF-81");

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
            await outputStream.WriteAsync(Definition.MagicNumberByte, cancellationToken);
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
        inputStream.Seek(Definition.MagicNumberLength, SeekOrigin.Current);
        var brotliStream = new BrotliStream(inputStream, CompressionMode.Decompress);
        return Task.FromResult<Stream>(brotliStream);
    }
}