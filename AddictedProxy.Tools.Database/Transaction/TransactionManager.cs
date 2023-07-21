using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Tools.Database.Transaction;

public class TransactionManager<TContext> : ITransactionManager<TContext> where TContext : DbContext
{
    private readonly TContext _dbContext;
    private readonly TransactionCounter _counter = new();

    public TransactionManager(TContext dbContext)
    {
        _dbContext = dbContext;
    }

    
    /// <summary>
    /// Wrap the code in a DB Transaction
    /// </summary>
    /// <param name="dbAction"></param>
    /// <param name="token"></param>
    public async Task WrapInTransactionAsync(Func<Task> dbAction, CancellationToken token)
    {
        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();
        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await BeginNestedAsync(token);
            await dbAction();
            await transaction.CommitAsync(token);
        });
    }

    /// <summary>
    /// Begin a transaction
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private async Task<ITransactionContainer> BeginNestedAsync(CancellationToken token)
    {
        var transaction = _dbContext.Database.CurrentTransaction ?? await _dbContext.Database.BeginTransactionAsync(token);
        _counter.NestedTransactionCount++;
        return new TransactionContainer(transaction, _counter);
    }
}