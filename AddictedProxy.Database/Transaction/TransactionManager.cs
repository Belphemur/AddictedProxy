using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Transaction;

public interface ITransactionManager
{
    /// <summary>
    /// Begin a transaction
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<ITransactionContainer> BeginNestedAsync(CancellationToken token);
}

public class TransactionManager<TContext> : ITransactionManager where TContext : DbContext
{
    private readonly TContext _dbContext;
    private readonly TransactionCounter _counter = new();

    public TransactionManager(TContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Begin a transaction
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<ITransactionContainer> BeginNestedAsync(CancellationToken token)
    {
        var transaction = _dbContext.Database.CurrentTransaction ?? await _dbContext.Database.BeginTransactionAsync(token);
        _counter.NestedTransactionCount++;
        return new TransactionContainer(transaction, _counter);
    }
    
}