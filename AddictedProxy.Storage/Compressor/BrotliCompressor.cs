using System.IO.Compression;

namespace AddictedProxy.Storage.Compressor;

public class BrotliCompressor : ICompressor
{

    public BrotliCompressor()
    {
    }

    /// <summary>
    /// Extension related to the compressor
    /// </summary>
    public string Extension => ".brotli";


    /// <summary>
    /// Compress bytes
    /// </summary>
    /// <param name="bytes">Bytes</param>
    /// <returns>Return compressed bytes</returns>
    public byte[] Compress(byte[] bytes)
    {
        using var memoryStream = new MemoryStream(bytes);
        using var brotliStream = new BrotliStream(memoryStream, CompressionLevel.Optimal);

        using var result = new MemoryStream();
        brotliStream.CopyTo(result);
        return result.ToArray();
    }

    /// <summary>
    /// Decompress bytes
    /// </summary>
    /// <param name="compressedBytes">Compressed bytes</param>
    /// <returns>Return uncompressed bytes</returns>
    public byte[] Decompress(byte[] compressedBytes)
    {
        using var memoryStream = new MemoryStream(compressedBytes);
        using var brotliStream = new BrotliStream(memoryStream, CompressionMode.Decompress);

        using var result = new MemoryStream();
        brotliStream.CopyTo(result);
        return result.ToArray();
    }

    /// <summary>
    /// Compress input stream to output stream
    /// </summary>
    /// <param name="inputStream">Input stream</param>
    /// <param name="outputStream">Output stream</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public async Task CompressAsync(Stream inputStream, Stream outputStream, CancellationToken cancellationToken = default)
    {
        await using var brotliStream = new BrotliStream(inputStream, CompressionLevel.Optimal);
        await brotliStream.CopyToAsync(outputStream, cancellationToken);
    }

    /// <summary>
    /// Decompress input stream to output stream
    /// </summary>
    /// <param name="inputStream">Input stream</param>
    /// <param name="outputStream">Output stream</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public async Task DecompressAsync(Stream inputStream, Stream outputStream, CancellationToken cancellationToken = default)
    {
        await using var brotliStream = new BrotliStream(inputStream, CompressionMode.Decompress);
        await brotliStream.CopyToAsync(outputStream, cancellationToken);
    }
}