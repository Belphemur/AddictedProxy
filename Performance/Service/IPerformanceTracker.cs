namespace Sentry.Performance.Service;

public interface IPerformanceTracker
{
    /// <summary>
    /// Start a transaction
    /// </summary>
    Model.ITransaction StartTransaction(string operation, string description);
}