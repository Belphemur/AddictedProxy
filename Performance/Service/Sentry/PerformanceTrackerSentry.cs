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
        switch (_currentSpan)
        {
            //If the current span isn't finished, create a child from it
            case { IsFinished: false }:
            {
                var span = _currentSpan.StartChild(operation, description);
                return _currentSpan = span;
            }
            //If the span IsFinished, then find the closest parent that isn't
            case { IsFinished: true }:
            {
                var span = _currentSpan.Parent;
                while (span is { IsFinished: true })
                {
                    span = span.Parent;
                }

                _currentSpan = span;
                return BeginNestedSpan(operation, description);
            }
            case null:
            {
                var span = _sentryHub.GetSpan();
                if (span == null)
                {
                    var transaction = _sentryHub.StartTransaction(operation, description);
                    _sentryHub.ConfigureScope(scope => { scope.Transaction = transaction; });
                    span = transaction;
                    _currentSpan = new SpanSentry(span, null);
                }
                else
                {
                    var childSpan = span.StartChild(operation, description);
                    _currentSpan = new SpanSentry(childSpan, new SpanSentry(span, null));
                }

                return _currentSpan;
            }
        }
    }
}