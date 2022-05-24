using Microsoft.EntityFrameworkCore.Storage;

namespace AddictedProxy.Database.Transaction;

public sealed class TransactionContainer : ITransactionContainer
{
    private readonly IDbContextTransaction _transaction;
    private readonly TransactionCounter _counter;
    private bool _committed;

    public TransactionContainer(IDbContextTransaction transaction, TransactionCounter counter)
    {
        _transaction = transaction;
        _counter = counter;
    }

    /// <summary>
    /// Commit the transaction
    /// </summary>
    /// <param name="token"></param>
    public async Task CommitAsync(CancellationToken token)
    {
        //Only commit if this is the only active transaction
        if (_counter.NestedTransactionCount != 1)
        {
            return;
        }

        await _transaction.CommitAsync(token);
        _committed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (--_counter.NestedTransactionCount != 0)
        {
            return;
        }

        if (!_committed)
        {
            await _transaction.RollbackAsync();
        }

        await _transaction.DisposeAsync();
    }
}