using EasyCompressor;

namespace AddictedProxy.Storage.Compressor;

public class ZstdCompressor : ICompressor
{
    private readonly EasyCompressor.ICompressor _compressor;

    public ZstdCompressor(EasyCompressor.ZstdCompressor compressor)
    {
        _compressor = compressor;
    }

    /// <summary>
    /// Extension related to the compressor
    /// </summary>
    public string Extension => ".zstd";


    /// <summary>
    /// Compress bytes
    /// </summary>
    /// <param name="bytes">Bytes</param>
    /// <returns>Return compressed bytes</returns>
    public byte[] Compress(byte[] bytes) => _compressor.Compress(bytes);

    /// <summary>
    /// Decompress bytes
    /// </summary>
    /// <param name="compressedBytes">Compressed bytes</param>
    /// <returns>Return uncompressed bytes</returns>
    public byte[] Decompress(byte[] compressedBytes) => _compressor.Decompress(compressedBytes);

    /// <summary>
    /// Compress input stream to output stream
    /// </summary>
    /// <param name="inputStream">Input stream</param>
    /// <param name="outputStream">Output stream</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public Task CompressAsync(Stream inputStream, Stream outputStream, CancellationToken cancellationToken = default) => _compressor.CompressAsync(inputStream, outputStream, cancellationToken);

    /// <summary>
    /// Decompress input stream to output stream
    /// </summary>
    /// <param name="inputStream">Input stream</param>
    /// <param name="outputStream">Output stream</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public Task DecompressAsync(Stream inputStream, Stream outputStream, CancellationToken cancellationToken = default) => _compressor.DecompressAsync(inputStream, outputStream, cancellationToken);
}