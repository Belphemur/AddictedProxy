namespace Locking;

public interface ILockContainer : IDisposable
{
    /// <summary>
    /// Wait for getting the lock
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <param name="token"></param>
    /// <returns>True if got the lock before <see cref="timeSpan"/> expires</returns>
    Task<bool> WaitAsync(TimeSpan timeSpan, CancellationToken token);
}
