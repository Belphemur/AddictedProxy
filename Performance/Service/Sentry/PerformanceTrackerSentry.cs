using Sentry.Performance.Model.Sentry;

namespace Sentry.Performance.Service.Sentry;

public class PerformanceTrackerSentry : IPerformanceTracker
{
    private TransactionSentry? _currentTransaction;

    /// <summary>
    /// Start a transaction.
    /// <remarks>If there is already a ongoing transaction in the scope, create a child from it.</remarks>
    /// </summary>
    public Model.ITransaction BeginNestedSpan(string operation, string description)
    {
        //If the current transaction isn't finished, create a child from it
        if (_currentTransaction is { IsFinished: false })
        {
            var transaction = new TransactionSentry(((SpanSentry)_currentTransaction.StartChild(operation, description)).InternalSpan, _currentTransaction);
            transaction.OnTransactionFinished += TransactionOnOnTransactionFinished;
            return _currentTransaction = transaction;
        }

        var currentTransaction = new TransactionSentry(SentrySdk.StartTransaction(operation, description), null);
        currentTransaction.OnTransactionFinished += TransactionOnOnTransactionFinished;
        return _currentTransaction = currentTransaction;
    }

    private void TransactionOnOnTransactionFinished(object sender, TransactionSentry.TransactionFinishedEvent e)
    {
        e.Transaction.OnTransactionFinished -= TransactionOnOnTransactionFinished;
        if (e.Transaction.SpanId != _currentTransaction?.SpanId || e.Transaction.Parent == null)
        {
            return;
        }

        _currentTransaction = new TransactionSentry(e.Transaction.Parent.InternalSpan, e.Transaction.Parent.Parent);
    }
}