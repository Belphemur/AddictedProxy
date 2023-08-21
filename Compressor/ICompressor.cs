﻿namespace Compressor;

public interface ICompressor
{
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