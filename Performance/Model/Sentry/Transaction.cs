namespace Sentry.Performance.Model.Sentry;

internal class Transaction : Span, ITransaction
{
    public Transaction(global::Sentry.ITransaction span) : base(span)
    {
    }
}