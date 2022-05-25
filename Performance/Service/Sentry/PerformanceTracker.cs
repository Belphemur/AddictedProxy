namespace Sentry.Performance.Service.Sentry;

public class PerformanceTracker : IPerformanceTracker
{
    private Model.ITransaction? _currentTransaction;

    /// <summary>
    /// Start a transaction.
    /// <remarks>If there is already a ongoing transaction in the scope, create a child from it.</remarks>
    /// </summary>
    public Model.ISpan StartNestedTransaction(string operation, string description)
    {
        //If the current transaction isn't finished, create a child from it
        if (_currentTransaction is { IsFinished: false })
        {
            return _currentTransaction.StartChild(operation, description);
        }

        return _currentTransaction = new Model.Sentry.Transaction(SentrySdk.StartTransaction(operation, description));
    }
}