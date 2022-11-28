using AddictedProxy.Controllers.Rest;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Model.Dto;
using AddictedProxy.Model.Responses;
using AddictedProxy.Model.Search;
using AddictedProxy.Services.Provider.Episodes;
using AddictedProxy.Services.Provider.Seasons;
using AddictedProxy.Services.Provider.Shows;
using AddictedProxy.Services.Provider.Subtitle.Jobs;
using AddictedProxy.Stats.Popularity.Model;
using AddictedProxy.Upstream.Service.Culture;
using Amazon.Runtime.Internal;
using Ardalis.Result;
using Job.Scheduler.AspNetCore.Builder;
using Job.Scheduler.Scheduler;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sentry.Performance.Service;
using ErrorResponse = AddictedProxy.Model.Responses.ErrorResponse;

namespace AddictedProxy.Services.Search;

public class SearchSubtitlesService : ISearchSubtitlesService
{
    private readonly IShowRefresher _showRefresher;
    private readonly CultureParser _cultureParser;
    private readonly IJobBuilder _jobBuilder;
    private readonly IJobScheduler _jobScheduler;
    private readonly ILogger<SearchSubtitlesService> _logger;
    private readonly IEpisodeRepository _episodeRepository;
    private readonly ISeasonRefresher _seasonRefresher;
    private readonly IEpisodeRefresher _episodeRefresher;
    private readonly IPerformanceTracker _performanceTracker;

    public SearchSubtitlesService(IShowRefresher showRefresher,
                                  CultureParser cultureParser,
                                  IJobBuilder jobBuilder,
                                  IJobScheduler jobScheduler,
                                  ILogger<SearchSubtitlesService> logger,
                                  IEpisodeRepository episodeRepository,
                                  ISeasonRefresher seasonRefresher,
                                  IEpisodeRefresher episodeRefresher,
                                  IPerformanceTracker performanceTracker)
    {
        _showRefresher = showRefresher;
        _cultureParser = cultureParser;
        _jobBuilder = jobBuilder;
        _jobScheduler = jobScheduler;
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

        var language = _cultureParser.FromString(request.LanguageIso);
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

        var episode = await _episodeRepository.GetEpisodeUntrackedAsync(show.Id, request.Season, request.Episode, token);

        if (episode == null)
        {
            var season = show.Seasons.FirstOrDefault(season => season.Number == request.Season);
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

            ScheduleJob(request, show);
            return new SubtitleFound(ArraySegment<Subtitle>.Empty, new EpisodeDto(request.Season, request.Episode, show.Name), language);
        }

        var matchingSubtitles = FindMatchingSubtitles(request, episode).ToArray();
        if (matchingSubtitles.Length == 0)
        {
            ScheduleJob(request, show);
        }

        return new SubtitleFound(matchingSubtitles, new EpisodeDto(episode), language);
    }

    private IEnumerable<Subtitle> FindMatchingSubtitles(SearchPayload payload, Episode episode)
    {
        using var _ = _performanceTracker.BeginNestedSpan("find-matching-subtitles", $"Episode [{episode.Id}] S{episode.Season}E{episode.Number}");
        var searchLanguage = _cultureParser.FromString(payload.LanguageIso)!;
        var search = episode.Subtitles
                            .Where(subtitle => subtitle.LanguageCodeIso3Letters == searchLanguage.ThreeLetterISOLanguageName || Equals(_cultureParser.FromString(subtitle.Language), searchLanguage));
        if (payload.FileName != null)
        {
            search = search.Where(subtitle => subtitle.Scene.ToLowerInvariant().Split('+', '.', '-').Any(version => payload.FileName.ToLowerInvariant().Contains(version)));
        }

        return search;
    }

    private void ScheduleJob(SearchPayload payload, TvShow show)
    {
        var job = _jobBuilder.Create<FetchSubtitlesJob>()
                             .Configure(subtitlesJob => { subtitlesJob.Data = new FetchSubtitlesJob.JobData(show.Id, payload.Season, payload.Episode, _cultureParser.FromString(payload.LanguageIso), payload.FileName); })
                             .Build();
        _jobScheduler.ScheduleJob(job);
    }
}