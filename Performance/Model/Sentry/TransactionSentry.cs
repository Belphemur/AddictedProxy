using Sentry.Performance.Service;

namespace Sentry.Performance.Model.Sentry;

internal class TransactionSentry : SpanSentry, ITransaction
{
    internal record TransactionFinishedEvent(TransactionSentry Transaction);

    public event EventHandler<TransactionFinishedEvent> OnTransactionFinished = null!;
    

    protected override void OnFinished()
    {
        OnTransactionFinished(this, new TransactionFinishedEvent(this));
    }

    public TransactionSentry(global::Sentry.ISpan internalSpan, SpanSentry? parent) : base(internalSpan, parent)
    {
    }
}