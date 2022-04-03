#region

using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Addic7ed;
using AddictedProxy.Services.Credentials;

#endregion

namespace AddictedProxy.Services.Saver;

public class AddictedSaver : IAddictedSaver
{
    private readonly IAddic7edClient _addic7EdClient;
    private readonly ICredentialsService _credentialsService;
    private readonly ITvShowRepository _tvShowRepository;

    public AddictedSaver(ITvShowRepository tvShowRepository, IAddic7edClient addic7EdClient, ICredentialsService credentialsService)
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
}