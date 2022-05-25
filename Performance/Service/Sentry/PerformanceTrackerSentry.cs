using Sentry.Performance.Model.Sentry;

namespace Sentry.Performance.Service.Sentry;

public class PerformanceTrackerSentry : IPerformanceTracker
{
    private readonly IHub _sentryHub;
    private SpanSentry? _currentSpan;

    public PerformanceTrackerSentry(IHub sentryHub)
    {
        _sentryHub = sentryHub;
    }

    /// <summary>
    /// Start a transaction.
    /// <remarks>If there is already a ongoing transaction in the scope, create a child from it.</remarks>
    /// </summary>
    public Model.ISpan BeginNestedSpan(string operation, string description)
    {
        //If the current transaction isn't finished, create a child from it
        if (_currentSpan is { IsFinished: false })
        {
            var span = _currentSpan.StartChild(operation, description);
            span.OnSpanFinished += OnSpanFinished;
            return _currentSpan = span;
        }

        var transaction = _sentryHub.StartTransaction(operation, description);
        var currentTransaction = new SpanSentry(transaction, null);
        
        _sentryHub.ConfigureScope(scope => { scope.Transaction = transaction; });
        
        currentTransaction.OnSpanFinished += OnSpanFinished;
        return _currentSpan = currentTransaction;
    }

    private void OnSpanFinished(object sender, SpanSentry.SpanFinishedEvent e)
    {
        if (e.Span.SpanId != _currentSpan?.SpanId || e.Span.Parent == null)
        {
            return;
        }

        _currentSpan = e.Span.Parent;
    }
}