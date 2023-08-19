using InversionOfControl.Model.Factory;

namespace AddictedProxy.Storage.Compressor.Factory;

public interface ICompressorService : IEnumService<CompressorType>
{
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