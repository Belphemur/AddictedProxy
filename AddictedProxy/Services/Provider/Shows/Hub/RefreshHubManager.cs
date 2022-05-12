using System.Collections.Concurrent;
using AddictedProxy.Controllers.Hub;

namespace AddictedProxy.Services.Provider.Shows.Hub;

public class RefreshHubManager : IRefreshHubManager
{
    private readonly RefreshHub _refreshHub;
    private readonly ILogger<RefreshHubManager> _logger;
    private readonly ConcurrentDictionary<Guid, ConcurrentBag<string>> _showToConnectionId = new();

    public RefreshHubManager(RefreshHub refreshHub, ILogger<RefreshHubManager> logger)
    {
        _refreshHub = refreshHub;
        _logger = logger;
    }

    /// <summary>
    /// Register a connectId for a specific show
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="connectionId"></param>
    public void Register(Guid showId, string connectionId)
    {
        _showToConnectionId.AddOrUpdate(showId, new ConcurrentBag<string>(new[] { connectionId }), (_, set) =>
        {
            set.Add(connectionId);
            return set;
        });
    }

    /// <summary>
    /// Send progress for given show
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="progress"></param>
    /// <param name="token"></param>
    public async Task SendProgressAsync(Guid showId, int progress, CancellationToken token)
    {
        if (!_showToConnectionId.TryGetValue(showId, out var connections))
        {
            _logger.LogInformation("Couldn't find connection for show: {showId}", showId);
            return;
        }

        await Task.WhenAll(connections.Select(connection => _refreshHub.SendProgressAsync(connection, showId, progress, token)));
    }

    public async Task SendShowRefreshedAsync(Guid showId, CancellationToken token)
    {
        if (!_showToConnectionId.TryGetValue(showId, out var connections))
        {
            _logger.LogInformation("Couldn't find connection for show: {showId}", showId);
            return;
        }

        await Task.WhenAll(connections.Select(connection => _refreshHub.SendRefreshDone(connection, showId, token)));
        _showToConnectionId.TryRemove(showId, out _);
    }
}