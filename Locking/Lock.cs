#region

using KeyedSemaphores;

#endregion

namespace Locking;

public static class Lock<T>
{
    /// <summary>
    /// Get a named lock
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static ILockContainer GetNamedLock(string name)
    {
        return new Container(KeyedSemaphore.GetOrCreate(GetLockKey(name)));
    }

    private static string GetLockKey(string key)
    {
        return $"{typeof(T).Name}_{key}";
    }

    private class Container : ILockContainer
    {
        private readonly IKeyedSemaphore<string> _semaphore;

        public Container(IKeyedSemaphore<string> semaphore)
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
            _semaphore.Dispose();
        }
    }
}