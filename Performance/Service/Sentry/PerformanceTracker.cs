namespace Sentry.Performance.Service.Sentry;

public class PerformanceTracker : IPerformanceTracker
{
    /// <summary>
    /// Start a transaction
    /// </summary>
    public Model.ITransaction StartTransaction(string operation, string description) => new Model.Sentry.Transaction(SentrySdk.StartTransaction(operation, description));
}