using AddictedProxy.Database;
using AddictedProxy.Model.Config;
using AddictedProxy.Services.Addic7ed;

namespace AddictedProxy.Services.Saver;

public class AddictedSaver : IAddictedSaver
{
    private readonly ITvShowRepository _tvShowRepository;
    private readonly IAddic7edClient _addic7EdClient;
    private readonly MainCreds _mainCreds;

    public AddictedSaver(ITvShowRepository tvShowRepository, IAddic7edClient addic7EdClient, MainCreds mainCreds)
    {
        _tvShowRepository = tvShowRepository;
        _addic7EdClient = addic7EdClient;
        _mainCreds = mainCreds;
    }

    public async Task RefreshShowsAsync(CancellationToken token)
    {
        var tvShows = await _addic7EdClient.GetTvShowsAsync(_mainCreds, token).ToArrayAsync(token);
        await _tvShowRepository.UpsertAsync(tvShows, token);
    }
}