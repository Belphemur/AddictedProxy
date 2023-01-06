using AddictedProxy.Storage.Store;

namespace AddictedProxy.Storage.Caching.Service;

public interface ICachedStorageProvider
{
    /// <summary>
    /// Store a file
    /// </summary>
    /// <param name="shardingKey">Key used for the distributed caching for choosing shard</param>
    /// <param name="filename"></param>
    /// <param name="inputStream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> StoreAsync(string shardingKey, string filename, Stream inputStream, CancellationToken cancellationToken);

    /// <summary>
    /// Download the file as a stream
    /// </summary>
    /// <param name="shardingKey">Key used for the distributed caching for choosing shard</param>
    /// <param name="filename"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Stream?> DownloadAsync(string shardingKey, string filename, CancellationToken cancellationToken);
}