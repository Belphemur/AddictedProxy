using System;
using System.Threading;
using System.Threading.Tasks;

namespace AddictedProxy.Services.Caching
{
    public interface ICachingService
    {
        /// <summary>
        /// Get or set in the cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objBuilder"></param>
        /// <param name="expiry">Does this entry expire ?</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetSet<T>(string key, Func<T> objBuilder, TimeSpan? expiry);

        /// <summary>
        /// Get or set in the cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objBuilder"></param>
        /// <param name="expiry">Does this entry expire ?</param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> GetSetAsync<T>(string key, Func<CancellationToken, Task<T>> objBuilder, TimeSpan? expiry, CancellationToken cancellationToken);
    }
}