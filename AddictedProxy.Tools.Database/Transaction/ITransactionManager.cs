using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Tools.Database.Transaction;

public interface ITransactionManager<TContext> where TContext : DbContext
{
    /// <summary>
    /// Wrap the code in a DB Transaction
    /// </summary>
    /// <param name="dbAction"></param>
    /// <param name="token"></param>
    Task WrapInTransactionAsync(Func<Task> dbAction, CancellationToken token);
}