using Performance.Model.Sentry;
using Sentry;

namespace Performance.Service.Sentry;

public class PerformanceTrackerSentry : IPerformanceTracker, IDisposable
{
    private SpanSentry? _currentSpan;
    private IDisposable? _sentryScope;

    /// <summary>
    /// Start a transaction.
    /// <remarks>If there is already a ongoing transaction in the scope, create a child from it.</remarks>
    /// </summary>
    public Model.ISpan BeginNestedSpan(string operation, string? description)
    {
        description ??= "N/A";
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
                var span = SentrySdk.GetSpan();
                if (span == null)
                {
                    _sentryScope = SentrySdk.PushScope();
                    var transaction = SentrySdk.StartTransaction(operation, description);
                    SentrySdk.ConfigureScope(scope => { scope.Transaction = transaction; });
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

    public void Dispose()
    {
        _currentSpan?.Dispose();
        _sentryScope?.Dispose();
    }
}