using AddictedProxy.Culture.Service;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Model.Dto;
using AddictedProxy.Model.Search;
using AddictedProxy.Services.Provider.Episodes;
using AddictedProxy.Services.Provider.Seasons;
using AddictedProxy.Services.Provider.Shows;
using AddictedProxy.Services.Provider.Subtitle.Jobs;
using Hangfire;
using Microsoft.AspNetCore.Http.HttpResults;
using Performance.Service;

namespace AddictedProxy.Services.Search.Subtitle;

public class SearchSubtitlesService : ISearchSubtitlesService
{
    private readonly IShowRefresher _showRefresher;
    private readonly ICultureParser _cultureParser;
    private readonly ILogger<SearchSubtitlesService> _logger;
    private readonly IEpisodeRepository _episodeRepository;
    private readonly ISeasonRefresher _seasonRefresher;
    private readonly IEpisodeRefresher _episodeRefresher;
    private readonly IPerformanceTracker _performanceTracker;

    public SearchSubtitlesService(IShowRefresher showRefresher,
                                  ICultureParser cultureParser,
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
    public async Task<Results<Ok<TvShow>, NotFound>> FindShowAsync(string showName, CancellationToken token)
    {
        var show = await _showRefresher.FindShowsAsync(showName, token).FirstOrDefaultAsync(token);
        return show == null ? TypedResults.NotFound() : TypedResults.Ok(show);
    }

    public async Task<Results<Ok<TvShow>, NotFound>> GetByShowUniqueIdAsync(Guid showUniqueId, CancellationToken token)
    {
        using var _ = _performanceTracker.BeginNestedSpan("get-show-uuid", $"Show {showUniqueId}");
        var show = await _showRefresher.GetShowByGuidAsync(showUniqueId, token);
        return show == null ? TypedResults.NotFound() : TypedResults.Ok(show);
    }

    /// <summary>
    /// Find subtitles
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<Results<Ok<SubtitleFound>, StatusCodeHttpResult>> FindSubtitlesAsync(SearchPayload request, CancellationToken token)
    {
        using var _ = _performanceTracker.BeginNestedSpan("find-subtitles", $"Show {request.Show.UniqueId}/{request.Show.Name}");

        var language = await _cultureParser.FromStringAsync(request.LanguageIso, token);
        if (language == null)
        {
            return TypedResults.StatusCode(423);
        }

        var show = request.Show;
        var season = show.Seasons.FirstOrDefault(season => season.Number == request.Season);

        var episode = await _episodeRepository.GetEpisodeUntrackedAsync(show.Id, request.Season, request.Episode, token);

        if (episode == null)
        {
            await TryScheduleJobAsync(request, show, season, language, token);
            return TypedResults.Ok(new SubtitleFound(ArraySegment<Database.Model.Shows.Subtitle>.Empty, new EpisodeDto(request.Season, request.Episode, show.Name), language));
        }

        var matchingSubtitles = await FindMatchingSubtitlesAsync(request, episode, token);
        //Only refresh if we couldn't find subtitle and the season list or episode/subtitle list needs to be refreshed
        if (matchingSubtitles.Length == 0)
        {
            await TryScheduleJobAsync(request, show, season, language, token);
        }

        return TypedResults.Ok(new SubtitleFound(matchingSubtitles, new EpisodeDto(episode), language));
    }

    private async Task<Database.Model.Shows.Subtitle[]> FindMatchingSubtitlesAsync(SearchPayload payload, Episode episode, CancellationToken token)
    {
        using var _ = _performanceTracker.BeginNestedSpan("find-matching-subtitles", $"Episode [{episode.Id}] S{episode.Season}E{episode.Number}");
        var searchLanguage = (await _cultureParser.FromStringAsync(payload.LanguageIso, token))!;
        var search = episode.Subtitles
                            .ToAsyncEnumerable()
                            .WhereAwait(async subtitle =>
                                subtitle.LanguageIsoCode == searchLanguage.Name || searchLanguage == await _cultureParser.FromStringAsync(subtitle.Language, token));

        return await search.ToArrayAsync(token);
    }

    private async Task TryScheduleJobAsync(SearchPayload payload, TvShow show, Season? season, Culture.Model.Culture language, CancellationToken token)
    {
        if (season == null && !await _seasonRefresher.IsShowNeedsRefreshAsync(show, token))
        {
            _logger.LogInformation("Don't need to refresh seasons of show {show} returning empty data", show.Name);
            return;
        }

        if (season != null && !await _episodeRefresher.IsSeasonNeedRefreshAsync(show, season, token))
        {
            _logger.LogInformation("Don't need to refresh episodes of {season} of show {show} returning empty data", payload.Season, show.Name);
            return;
        }


        var jobData = new FetchSubtitlesJob.JobData(show.Id, payload.Season, payload.Episode, language, payload.FileName);

        BackgroundJob.Enqueue<FetchSubtitlesJob>(subtitlesJob => subtitlesJob.ExecuteAsync(jobData, null, default));
    }
}