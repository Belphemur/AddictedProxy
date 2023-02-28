namespace Performance.Service;

public interface IPerformanceTracker
{
    /// <summary>
    /// Start a transaction.
    /// <remarks>If there is already a ongoing transaction in the scope, create a child from it.</remarks>
    /// </summary>
    Model.ISpan BeginNestedSpan(string operation, string? description = null);
}