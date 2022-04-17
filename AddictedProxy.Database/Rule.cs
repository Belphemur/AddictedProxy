#region

using AddictedProxy.Database.Model.Shows;
using Z.BulkOperations;

#endregion

namespace AddictedProxy.Database;

public static class Rule
{
    public static Action<BulkOperation<T>> AvoidUpdateDiscoveredField<T>() where T : class, IDiscoverableObject
    {
        return operation => operation.IgnoreOnUpdateExpression = o => o.Discovered;
    }
}