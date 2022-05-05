#region

using System.Globalization;
using AddictedProxy.Database.Model.Credentials;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Services.Credentials;
using AddictedProxy.Services.Culture;
using AddictedProxy.Upstream.Service;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Locking;

#endregion

namespace AddictedProxy.Services.Provider.Subtitle.Job;

public class FetchSubtitlesJob : IJob
{
    private readonly IAddic7edClient _client;
    private readonly ICredentialsService _credentialsService;
    private readonly CultureParser _cultureParser;
    private readonly IEpisodeRepository _episodeRepository;

    private readonly ILogger<FetchSubtitlesJob> _logger;
    private readonly ISeasonRepository _seasonRepository;
    private readonly ITvShowRepository _tvShowRepository;


    public FetchSubtitlesJob(ILogger<FetchSubtitlesJob> logger,
                             ICredentialsService credentialsService,
                             IAddic7edClient client,
                             CultureParser cultureParser,
                             IEpisodeRepository episodeRepository,
                             ISeasonRepository seasonRepository,
                             ITvShowRepository tvShowRepository)
    {
        _logger = logger;
        _credentialsService = credentialsService;
        _client = client;
        _cultureParser = cultureParser;
        _episodeRepository = episodeRepository;
        _seasonRepository = seasonRepository;
        _tvShowRepository = tvShowRepository;
    }

    public TimeSpan TimeBetweenChecks { get; } = TimeSpan.FromMinutes(30);
    public JobData Data { get; set; }


    public async Task ExecuteAsync(CancellationToken token)
    {
        using var scope = _logger.BeginScope(Data.ScopeName);
        using var namedLock = Lock<FetchSubtitlesJob>.GetNamedLock(Data.Key);
        if (!await namedLock.WaitAsync(TimeSpan.Zero, token))
        {
            _logger.LogInformation("Lock for {key} already taken", Data.Key);
            return;
        }

        var show = Data.Show;
        var season = show.Seasons.FirstOrDefault(season => season.Number == Data.Season);

        await using var credentials = await _credentialsService.GetLeastUsedCredsAsync(token);


        if (season == null && (show.LastSeasonRefreshed == null || DateTime.UtcNow - show.LastSeasonRefreshed >= TimeBetweenChecks))
        {
            var maxSeason = show.Seasons.Any() ? show.Seasons.Max(s => s.Number) : 0;
            if (show.Seasons.Any() && Data.Season - maxSeason > 1)
            {
                _logger.LogInformation("{season} is too far in the future.", Data.Key);

                return;
            }

            var seasons = (await _client.GetSeasonsAsync(credentials.AddictedUserCredentials, show, token)).ToArray();
            await _seasonRepository.UpsertSeason(seasons, token);
            show.LastSeasonRefreshed = DateTime.UtcNow;
            await _tvShowRepository.UpdateShowAsync(show, token);
            season = await _seasonRepository.GetSeasonForShow(show.Id, Data.Season, token);
        }

        if (season == null)
        {
            _logger.LogInformation("Couldn't find season {season} for show {showName}", Data.Season, Data.Show.Name);
            return;
        }

        var episode = await _episodeRepository.GetEpisodeUntrackedAsync(show.Id, season.Number, Data.Episode, token);

        var episodesRefreshed = season.LastRefreshed != null && DateTime.UtcNow - season.LastRefreshed <= TimeBetweenChecks;
        if (episode == null && !episodesRefreshed)
        {
            episode = await RefreshSubtitlesAsync(credentials.AddictedUserCredentials, season, true, token);
            episodesRefreshed = true;
        }

        if (episode == null)
        {
            _logger.LogInformation("Couldn't find episode S{season}E{episode} for show {showName}", Data.Season, Data.Episode, Data.Show.Name);

            return;
        }

        var matchingSubtitles = HasMatchingSubtitle(episode);

        var latestDiscovered = episode.Subtitles.Max(subtitle => subtitle.Discovered);

        if (matchingSubtitles || episodesRefreshed || DateTime.UtcNow - latestDiscovered > TimeSpan.FromDays(180))
        {
            _logger.LogInformation("Matching subtitles found or episode was already refreshed");
            return;
        }

        await RefreshSubtitlesAsync(credentials.AddictedUserCredentials, season, false, token);
    }

    public Task OnFailure(JobException exception)
    {
        using var scope = _logger.BeginScope(Data.ScopeName);
        _logger.LogError(exception, "Fetching subtitles info");
        return Task.CompletedTask;
    }

    public IRetryAction FailRule { get; } = new ExponentialBackoffRetry(TimeSpan.FromSeconds(5), 5);
    public TimeSpan? MaxRuntime { get; }

    private async Task<Episode?> RefreshSubtitlesAsync(AddictedUserCredentials credentials, Season season, bool reloadEpisode, CancellationToken token)
    {
        var episodes = await _client.GetEpisodesAsync(credentials, Data.Show, season.Number, token);
        await _episodeRepository.UpsertEpisodes(episodes, token);
        season.LastRefreshed = DateTime.UtcNow;
        await _seasonRepository.UpdateSeasonAsync(season, token);
        if (reloadEpisode)
        {
            return await _episodeRepository.GetEpisodeUntrackedAsync(Data.Show.Id, season.Number, Data.Episode, token);
        }

        return null;
    }

    private bool HasMatchingSubtitle(Episode episode)
    {
        var list = episode.Subtitles
                          .Where(subtitle => Equals(_cultureParser.FromString(subtitle.Language), Data.Language));
        if (Data.FileName != null)
        {
            list = list.Where(subtitle => subtitle.Scene.ToLowerInvariant().Split('+', '.', '-').Any(version => Data.FileName.ToLowerInvariant().Contains(version)));
        }

        return list.Any();
    }

    public record JobData(TvShow Show, int Season, int Episode, CultureInfo? Language, string? FileName)
    {
        public string Key => $"{Show.Id}-{Season}";
        public string ScopeName => $"{Show.Name} S{Season}E{Episode} ({Language}) [{FileName}]";
    }
}