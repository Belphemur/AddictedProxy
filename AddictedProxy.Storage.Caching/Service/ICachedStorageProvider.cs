using AddictedProxy.Storage.Store;

namespace AddictedProxy.Storage.Caching.Service;

public interface ICachedStorageProvider
{
    /// <summary>
    ///  Get the file from the cache or get it from the <see cref="IStorageProvider" /> and store it in the cache
    /// </summary>
    /// <param name="shardingKey"></param>
    /// <param name="filename"></param>
    /// <param name="dataGet"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Stream?> GetSertAsync(string shardingKey, string filename, Func<CancellationToken, Task<Stream?>> dataGet, CancellationToken cancellationToken);
}