namespace Compressor;

public interface ICompressor
{
    /// <summary>
    /// Get the full file name in the storage
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [Obsolete("Will always return the same value as the input parameter + .brotli for backward compatibility. Shouldn't be used anymore, magic number is used to recognize the compressor.")]
    public string GetFileName(string file);

    /// <summary>
    /// Compress input stream to output stream
    /// </summary>
    /// <param name="inputStream">Input stream</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task<Stream> CompressAsync(Stream inputStream, CancellationToken cancellationToken);

    /// <summary>
    /// Decompress input stream to output stream
    /// </summary>
    /// <param name="inputStream">Input stream</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task<Stream> DecompressAsync(Stream inputStream, CancellationToken cancellationToken);
}