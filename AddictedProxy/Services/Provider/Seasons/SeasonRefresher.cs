using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Model.Crendentials;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Upstream.Service;

namespace AddictedProxy.Services.Provider.Seasons;

public class SeasonRefresher : ISeasonRefresher
{
    private readonly ILogger<SeasonRefresher> _logger;
    private readonly ITvShowRepository _tvShowRepository;
    private readonly IAddic7edClient _addic7EdClient;
    private readonly ICredentialsService _credentialsService;
    private readonly ISeasonRepository _seasonRepository;

    public SeasonRefresher(ILogger<SeasonRefresher> logger, ITvShowRepository tvShowRepository, IAddic7edClient addic7EdClient, ICredentialsService credentialsService, ISeasonRepository seasonRepository)
    {
        _logger = logger;
        _tvShowRepository = tvShowRepository;
        _addic7EdClient = addic7EdClient;
        _credentialsService = credentialsService;
        _seasonRepository = seasonRepository;
    }

    public async Task<Season?> GetRefreshSeasonAsync(TvShow show, int seasonNumber, TimeSpan timeBetweenChecks, CancellationToken token)
    {
        var season = show.Seasons.FirstOrDefault(season => season.Number == seasonNumber);
        await using var credentials = await _credentialsService.GetLeastUsedCredsAsync(token);


        //Check if we need to refresh because enough time has passed
        if (season != null && show.LastSeasonRefreshed != null && !(DateTime.UtcNow - show.LastSeasonRefreshed >= timeBetweenChecks))
        {
            return season;
        }

        var maxSeason = show.Seasons.Any() ? show.Seasons.Max(s => s.Number) : 0;
        if (show.Seasons.Any() && seasonNumber - maxSeason > 1)
        {
            _logger.LogInformation("{season} is too far in the future.", seasonNumber);

            return season;
        }

        var _ = await RefreshSeasonsAsync(show, credentials, token);
        return await _seasonRepository.GetSeasonForShow(show.Id, seasonNumber, token);
    }


    public async Task<TvShow> RefreshSeasonsAsync(TvShow show, CredsContainer credentials, CancellationToken token)
    {
        var seasons = (await _addic7EdClient.GetSeasonsAsync(credentials.AddictedUserCredentials, show, token)).ToArray();
        await _seasonRepository.UpsertSeason(seasons, token);
        show.LastSeasonRefreshed = DateTime.UtcNow;
        await _tvShowRepository.UpdateShowAsync(show, token);
        return (await _tvShowRepository.GetByIdAsync(show.Id, token))!;
    }
}