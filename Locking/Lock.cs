namespace Locking;

public static class Lock<T>
{
    private static readonly LockDictionary Locks = LockDictionary.Instance;
    private static readonly object CacheLock = new();

    public class Container : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;

        public Container(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }

        /// <summary>
        /// Wait for getting the lock
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="token"></param>
        /// <returns>True if got the lock before <see cref="timeSpan"/> expires</returns>
        public Task<bool> WaitAsync(TimeSpan timeSpan, CancellationToken token)
        {
            return _semaphore.WaitAsync(timeSpan, token);
        }

        public void Dispose()
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Get a named lock
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Container GetNamedLock(string name)
    {
        lock (CacheLock)
        {
            var lockKey = GetLockKey(name);
            if (!Locks.ContainsKey(lockKey))
            {
                Locks.TryAdd(lockKey, new SemaphoreSlim(1, 1));
            }

            return new Container(Locks[lockKey]);
        }
    }

    private static string GetLockKey(string key)
    {
        return $"{typeof(T).Name}_{key}";
    }
}