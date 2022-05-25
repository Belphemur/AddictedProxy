using Sentry.Performance.Model.Sentry;

namespace Sentry.Performance.Service.Sentry;

public class PerformanceTrackerSentry : IPerformanceTracker
{
    private SpanSentry? _currentTransaction;

    /// <summary>
    /// Start a transaction.
    /// <remarks>If there is already a ongoing transaction in the scope, create a child from it.</remarks>
    /// </summary>
    public Model.ISpan BeginNestedSpan(string operation, string description)
    {
        //If the current transaction isn't finished, create a child from it
        if (_currentTransaction is { IsFinished: false })
        {
            var transaction = _currentTransaction.StartChild(operation, description);
            transaction.OnSpanFinished += TransactionOnOnTransactionFinished;
            return _currentTransaction = transaction;
        }

        var currentTransaction = new SpanSentry(SentrySdk.StartTransaction(operation, description), null);
        currentTransaction.OnSpanFinished += TransactionOnOnTransactionFinished;
        return _currentTransaction = currentTransaction;
    }

    private void TransactionOnOnTransactionFinished(object sender, SpanSentry.SpanFinishedEvent e)
    {
        if (e.Span.SpanId != _currentTransaction?.SpanId || e.Span.Parent == null)
        {
            return;
        }

        _currentTransaction = e.Span.Parent;
    }
}