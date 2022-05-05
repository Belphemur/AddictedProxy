#region

using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Upstream.Service;

#endregion

namespace AddictedProxy.Services.Provider.Show;

public class ShowProvider : IShowProvider
{
    private readonly IAddic7edClient _addic7EdClient;
    private readonly ICredentialsService _credentialsService;
    private readonly ITvShowRepository _tvShowRepository;

    public ShowProvider(ITvShowRepository tvShowRepository, IAddic7edClient addic7EdClient, ICredentialsService credentialsService)
    {
        _tvShowRepository = tvShowRepository;
        _addic7EdClient = addic7EdClient;
        _credentialsService = credentialsService;
    }

    public async Task RefreshShowsAsync(CancellationToken token)
    {
        await using var credentials = await _credentialsService.GetLeastUsedCredsAsync(token);
        var tvShows = await _addic7EdClient.GetTvShowsAsync(credentials.AddictedUserCredentials, token).ToArrayAsync(token);

        await _tvShowRepository.UpsertRefreshedShowsAsync(tvShows, token);
    }

    public IAsyncEnumerable<TvShow> FindShowsAsync(string search, CancellationToken token)
    {
        return _tvShowRepository.FindAsync(search, token);
    }

    public Task<TvShow?> GetShowByIdAsync(long id, CancellationToken cancellationToken)
    {
        return _tvShowRepository.GetByIdAsync(id, cancellationToken);
    }
}