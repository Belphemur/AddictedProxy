using System;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace AddictedProxy.Services.Caching
{
    public class CachingService : ICachingService, IDisposable
    {
        private readonly MemoryCache _memoryCache;

        public CachingService(MemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Get or set in the cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objBuilder"></param>
        /// <param name="expiry">Does this entry expire ?</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSet<T>(string key, Func<T> objBuilder, TimeSpan? expiry)
        {
            var inCache = _memoryCache.Get(key);
            if (inCache != null)
            {
                return (T) inCache;
            }

            var toCache = objBuilder();
            return Cache(key, toCache, expiry);
        }

        private T Cache<T>(string key, T toCache, TimeSpan? expiry)
        {
            var cachePolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now + (expiry ?? TimeSpan.Zero),
                RemovedCallback = async arguments =>
                {
                    switch (arguments.CacheItem.Value)
                    {
                        case IDisposable disposable:
                            disposable.Dispose();
                            break;
                        case IAsyncDisposable disposable:
                            await disposable.DisposeAsync();
                            break;
                    }
                }
            };
            var cacheEntry = new CacheItem(key, toCache);
            _memoryCache.Add(cacheEntry, cachePolicy);
            return toCache;
        }

        /// <summary>
        /// Get or set in the cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objBuilder"></param>
        /// <param name="expiry">Does this entry expire ?</param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> GetSetAsync<T>(string key, Func<CancellationToken, Task<T>> objBuilder, TimeSpan? expiry, CancellationToken cancellationToken)
        {
            
            var inCache = _memoryCache.Get(key);
            if (inCache != null)
            {
                return (T) inCache;
            }

            return Cache(key, await objBuilder(cancellationToken), expiry);
        }

        public void Dispose()
        {
            _memoryCache?.Dispose();
        }
    }
}