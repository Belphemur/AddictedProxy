using AsyncKeyedLock;

namespace Locking;

public static class Lock<T>
{
    private static readonly AsyncKeyedLocker<string> AsyncKeyedLocker = new();

    public static AsyncKeyedLockReleaser<string> GetLockReleaser(string key)
    {
        return AsyncKeyedLocker.GetOrAdd($"{typeof(T).Name}_{key}");
    }
}