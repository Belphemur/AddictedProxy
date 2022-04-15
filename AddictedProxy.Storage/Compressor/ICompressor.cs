namespace AddictedProxy.Storage.Compressor;

public interface ICompressor
{
    /// <summary>
    /// Extension related to the compressor
    /// </summary>
    string Extension { get; }

    /// <summary>
    /// Compress bytes
    /// </summary>
    /// <param name="bytes">Bytes</param>
    /// <returns>Return compressed bytes</returns>
    byte[] Compress(byte[] bytes);

    /// <summary>
    /// Decompress bytes
    /// </summary>
    /// <param name="compressedBytes">Compressed bytes</param>
    /// <returns>Return uncompressed bytes</returns>
    byte[] Decompress(byte[] compressedBytes);

    /// <summary>
    /// Compress input stream to output stream
    /// </summary>
    /// <param name="inputStream">Input stream</param>
    /// <param name="outputStream">Output stream</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task CompressAsync(Stream inputStream, Stream outputStream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Decompress input stream to output stream
    /// </summary>
    /// <param name="inputStream">Input stream</param>
    /// <param name="outputStream">Output stream</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task DecompressAsync(Stream inputStream, Stream outputStream, CancellationToken cancellationToken = default);
}