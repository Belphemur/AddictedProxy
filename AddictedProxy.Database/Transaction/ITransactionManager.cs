namespace AddictedProxy.Database.Transaction;

public interface ITransactionManager
{
    /// <summary>
    /// Wrap the code in a DB Transaction
    /// </summary>
    /// <param name="dbAction"></param>
    /// <param name="token"></param>
    Task WrapInTransactionAsync(Func<Task> dbAction, CancellationToken token);
}