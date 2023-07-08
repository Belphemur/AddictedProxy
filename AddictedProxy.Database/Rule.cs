#region

using AddictedProxy.Database.Model.Shows;
using RT.Comb;
using Z.BulkOperations;

#endregion

namespace AddictedProxy.Database;

public static class Rule
{
    private static readonly PostgreSqlCombProvider PostgreSqlCombProvider = new PostgreSqlCombProvider(new UnixDateTimeStrategy());

    public static Action<BulkOperation<T>> AvoidUpdateDiscoveredField<T>() where T : class, IDiscoverableObject
    {
        return operation => operation.IgnoreOnUpdateExpression = o => o.Discovered;
    }

    /// <summary>
    /// Generate a UUIDv7 for Postgres
    /// </summary>
    /// <returns></returns>
    public static Guid GenerateUuidv7Postgres()
    {
        return PostgreSqlCombProvider.Create();
    }
}