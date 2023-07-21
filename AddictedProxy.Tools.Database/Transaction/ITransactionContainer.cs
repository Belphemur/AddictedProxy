namespace AddictedProxy.Tools.Database.Transaction;

public interface ITransactionContainer : IAsyncDisposable
{
    /// <summary>
    /// Commit the transaction
    /// </summary>
    /// <param name="token"></param>
    Task CommitAsync(CancellationToken token);
}