namespace AddictedProxy.Services.Provider.Shows.Hub;

public interface IRefreshHubManager
{
    /// <summary>
    /// Register a connectId for a specific show
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="connectionId"></param>
    void Register(Guid showId, string connectionId);

    /// <summary>
    /// Send progress for given show
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="progress"></param>
    /// <param name="token"></param>
    Task SendProgressAsync(Guid showId, int progress, CancellationToken token);

    Task SendShowRefreshedAsync(Guid showId, CancellationToken token);
}