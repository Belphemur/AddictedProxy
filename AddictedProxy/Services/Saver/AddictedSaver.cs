using AddictedProxy.Database.Repositories;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Addic7ed;
using AddictedProxy.Services.Addic7ed.EnvVar.Credentials;

namespace AddictedProxy.Services.Saver;

public class AddictedSaver : IAddictedSaver
{
    private readonly IAddic7edClient _addic7EdClient;
    private readonly MainCreds _mainCreds;
    private readonly ITvShowRepository _tvShowRepository;

    public AddictedSaver(ITvShowRepository tvShowRepository, IAddic7edClient addic7EdClient, MainCreds mainCreds)
    {
        _tvShowRepository = tvShowRepository;
        _addic7EdClient = addic7EdClient;
        _mainCreds = mainCreds;
    }

    public async Task RefreshShowsAsync(CancellationToken token)
    {
        var tvShows = await _addic7EdClient.GetTvShowsAsync(_mainCreds, token).ToArrayAsync(token);
        await _tvShowRepository.UpsertRefreshedShowsAsync(tvShows, token);
    }
}