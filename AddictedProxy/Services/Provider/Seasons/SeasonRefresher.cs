using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Model.Crendentials;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Services.Provider.Config;
using AddictedProxy.Upstream.Service;
using Microsoft.Extensions.Options;

namespace AddictedProxy.Services.Provider.Seasons;

public class SeasonRefresher : ISeasonRefresher
{
    private readonly ILogger<SeasonRefresher> _logger;
    private readonly ITvShowRepository _tvShowRepository;
    private readonly IAddic7edClient _addic7EdClient;
    private readonly ICredentialsService _credentialsService;
    private readonly ISeasonRepository _seasonRepository;
    private readonly IOptions<RefreshConfig> _refreshConfig;

    public SeasonRefresher(ILogger<SeasonRefresher> logger,
                           ITvShowRepository tvShowRepository,
                           IAddic7edClient addic7EdClient,
                           ICredentialsService credentialsService,
                           ISeasonRepository seasonRepository,
                           IOptions<RefreshConfig> refreshConfig)
    {
        _logger = logger;
        _tvShowRepository = tvShowRepository;
        _addic7EdClient = addic7EdClient;
        _credentialsService = credentialsService;
        _seasonRepository = seasonRepository;
        _refreshConfig = refreshConfig;
    }

    public async Task<Season?> GetRefreshSeasonAsync(TvShow show, int seasonNumber, CancellationToken token)
    {
        var season = show.Seasons.FirstOrDefault(season => season.Number == seasonNumber);


        //Check if we need to refresh because enough time has passed
        if (season != null && show.LastSeasonRefreshed != null && !(DateTime.UtcNow - show.LastSeasonRefreshed >= _refreshConfig.Value.SeasonRefresh))
        {
            _logger.LogInformation("Don't need to refresh season {number} of {show}", seasonNumber, show.Name);
            return season;
        }

        var maxSeason = show.Seasons.Any() ? show.Seasons.Max(s => s.Number) : 0;
        if (show.Seasons.Any() && seasonNumber - maxSeason > 1)
        {
            _logger.LogInformation("{season} is too far in the future.", seasonNumber);

            return season;
        }

        await RefreshSeasonsAsync(show, true, token);
        return await _seasonRepository.GetSeasonForShow(show.Id, seasonNumber, token);
    }

    public async Task RefreshSeasonsAsync(TvShow show, bool force = false, CancellationToken token = default)
    {
        await using var credentials = await _credentialsService.GetLeastUsedCredsAsync(token);

        if (!force && show.LastSeasonRefreshed != null && !(DateTime.UtcNow - show.LastSeasonRefreshed >= _refreshConfig.Value.SeasonRefresh))
        {
            _logger.LogInformation("Don't need to refresh seasons of {show}", show.Name);
            return;
        }

        var seasons = (await _addic7EdClient.GetSeasonsAsync(credentials.AddictedUserCredentials, show, token)).ToArray();
        await _seasonRepository.UpsertSeason(seasons, token);
        show.LastSeasonRefreshed = DateTime.UtcNow;
        await _tvShowRepository.UpdateShowAsync(show, token);
        _logger.LogInformation("Fetched {number} seasons of {show}", seasons.Length, show.Name);
    }
}