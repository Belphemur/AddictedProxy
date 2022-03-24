using AddictedProxy.Model.Shows;
using Z.BulkOperations;

namespace AddictedProxy.Database;

public static class Rule
{
    public static Action<BulkOperation<T>> AvoidUpdateDiscoveredField<T>() where T : class, IDiscoverableObject => operation => operation.IgnoreOnUpdateExpression = o => o.Discovered;
} 