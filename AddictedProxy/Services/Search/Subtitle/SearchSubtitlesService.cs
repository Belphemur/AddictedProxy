using AddictedProxy.Culture.Service;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Model.Dto;
using AddictedProxy.Model.Search;
using AddictedProxy.Services.Provider.Episodes;
using AddictedProxy.Services.Provider.Seasons;
using AddictedProxy.Services.Provider.Shows;
using AddictedProxy.Services.Provider.Subtitle.Jobs;
using Ardalis.Result;
using Hangfire;
using Sentry.Performance.Service;

namespace AddictedProxy.Services.Search;

public class SearchSubtitlesService : ISearchSubtitlesService
{
    private readonly IShowRefresher _showRefresher;
    private readonly CultureParser _cultureParser;
    private readonly ILogger<SearchSubtitlesService> _logger;
    private readonly IEpisodeRepository _episodeRepository;
    private readonly ISeasonRefresher _seasonRefresher;
    private readonly IEpisodeRefresher _episodeRefresher;
    private readonly IPerformanceTracker _performanceTracker;

    public SearchSubtitlesService(IShowRefresher showRefresher,
                                  CultureParser cultureParser,
                                  ILogger<SearchSubtitlesService> logger,
                                  IEpisodeRepository episodeRepository,
                                  ISeasonRefresher seasonRefresher,
                                  IEpisodeRefresher episodeRefresher,
                                  IPerformanceTracker performanceTracker)
    {
        _showRefresher = showRefresher;
        _cultureParser = cultureParser;
        _logger = logger;
        _episodeRepository = episodeRepository;
        _seasonRefresher = seasonRefresher;
        _episodeRefresher = episodeRefresher;
        _performanceTracker = performanceTracker;
    }

    /// <summary>
    /// Find a show
    /// </summary>
    /// <param name="showName"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<Result<TvShow>> FindShowAsync(string showName, CancellationToken token)
    {
        var show = await _showRefresher.FindShowsAsync(showName, token).FirstOrDefaultAsync(token);
        return show == null ? Result.NotFound($"Couldn't find show {showName}") : show;
    }

    public async Task<Result<TvShow>> GetByShowUniqueIdAsync(Guid showUniqueId, CancellationToken token)
    {
        using var _ = _performanceTracker.BeginNestedSpan("get-show-uuid", $"Show {showUniqueId}");
        var show = await _showRefresher.GetShowByGuidAsync(showUniqueId, token);
        return show == null ? Result.NotFound($"Couldn't find show {showUniqueId}") : show;
    }

    /// <summary>
    /// Find subtitles
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<Result<SubtitleFound>> FindSubtitlesAsync(SearchPayload request, CancellationToken token)
    {
        using var _ = _performanceTracker.BeginNestedSpan("find-subtitles", $"Show {request.Show.UniqueId}/{request.Show.Name}");

        var language = await _cultureParser.FromStringAsync(request.LanguageIso, token);
        if (language == null)
        {
            return Result<SubtitleFound>.Invalid(new List<ValidationError>
            {
                new()
                {
                    Identifier = "language",
                    ErrorMessage = $"Couldn't parse language {request.LanguageIso}",
                    Severity = ValidationSeverity.Error
                }
            });
        }

        var show = request.Show;
        var season = show.Seasons.FirstOrDefault(season => season.Number == request.Season);

        var episode = await _episodeRepository.GetEpisodeUntrackedAsync(show.Id, request.Season, request.Episode, token);

        if (episode == null)
        {
            if (season == null && !_seasonRefresher.IsShowNeedsRefresh(show))
            {
                _logger.LogInformation("Don't need to refresh seasons of show {show} returning empty data", show.Name);
                return new SubtitleFound(ArraySegment<Subtitle>.Empty, new EpisodeDto(request.Season, request.Episode, show.Name), language);
            }

            if (season != null && !_episodeRefresher.IsSeasonNeedRefresh(show, season))
            {
                _logger.LogInformation("Don't need to refresh episodes of {season} of show {show} returning empty data", request.Season, show.Name);
                return new SubtitleFound(ArraySegment<Subtitle>.Empty, new EpisodeDto(request.Season, request.Episode, show.Name), language);
            }

            ScheduleJob(request, show, language);
            return new SubtitleFound(ArraySegment<Subtitle>.Empty, new EpisodeDto(request.Season, request.Episode, show.Name), language);
        }

        var matchingSubtitles = await FindMatchingSubtitlesAsync(request, episode, token);
        //Only refresh if we couldn't find subtitle and the season list or episode/subtitle list needs to be refreshed
        if (matchingSubtitles.Length == 0 && (_seasonRefresher.IsShowNeedsRefresh(show) || _episodeRefresher.IsSeasonNeedRefresh(show, season!)))
        {
            ScheduleJob(request, show, language);
        }

        return new SubtitleFound(matchingSubtitles, new EpisodeDto(episode), language);
    }

    private async Task<Subtitle[]> FindMatchingSubtitlesAsync(SearchPayload payload, Episode episode, CancellationToken token)
    {
        using var _ = _performanceTracker.BeginNestedSpan("find-matching-subtitles", $"Episode [{episode.Id}] S{episode.Season}E{episode.Number}");
        var searchLanguage = (await _cultureParser.FromStringAsync(payload.LanguageIso, token))!;
        var search = episode.Subtitles
                            .ToAsyncEnumerable()
                            .WhereAwait(async subtitle =>
                                subtitle.LanguageCodeIso3Letters == searchLanguage.ThreeLetterISOLanguageName || searchLanguage == await _cultureParser.FromStringAsync(subtitle.Language, token));

        return await search.ToArrayAsync(token);
    }

    private void ScheduleJob(SearchPayload payload, TvShow show, Culture.Model.Culture language)
    {
        var jobData = new FetchSubtitlesJob.JobData(show.Id, payload.Season, payload.Episode, language, payload.FileName);

        BackgroundJob.Enqueue<FetchSubtitlesJob>(subtitlesJob => subtitlesJob.ExecuteAsync(jobData, default));
    }
}