using Microsoft.EntityFrameworkCore.Storage;

namespace AddictedProxy.Database.Transaction;

public sealed class TransactionContainer : ITransactionContainer
{
    private readonly IDbContextTransaction _transaction;
    private readonly TransactionCounter _counter;
    private bool _commited;

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
        //Only transaction
        if (_counter.NestedTransactionCount == 1)
        {
            await _transaction.CommitAsync(token);
            _commited = true;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (--_counter.NestedTransactionCount != 0)
        {
            return;
        }

        if (!_commited)
        {
            await _transaction.RollbackAsync();
        }

        await _transaction.DisposeAsync();
    }
}