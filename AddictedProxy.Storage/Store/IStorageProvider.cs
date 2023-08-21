namespace AddictedProxy.Storage.Store;

public interface IStorageProvider
{
    /// <summary>
    /// Store a file
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="inputStream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> StoreAsync(string filename, Stream inputStream, CancellationToken cancellationToken);

    /// <summary>
    /// Download the file as a stream
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Stream?> DownloadAsync(string filename, CancellationToken cancellationToken);

    /// <summary>
    /// Delete the specific file
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="cancellationToken"></param>
    Task DeleteAsync(string filename, CancellationToken cancellationToken);
}