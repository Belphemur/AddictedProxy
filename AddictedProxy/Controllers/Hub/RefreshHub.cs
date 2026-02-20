using System.Runtime.CompilerServices;
using AddictedProxy.Controllers.Rest;
using AddictedProxy.Culture.Service;
using AddictedProxy.Database.Repositories.Shows;
using AddictedProxy.Model.Dto;
using AddictedProxy.Services.Details;
using AddictedProxy.Services.Provider.Shows;
using AddictedProxy.Services.Provider.Shows.Jobs;
using Hangfire;
using Microsoft.AspNetCore.SignalR;

namespace AddictedProxy.Controllers.Hub;

public class RefreshHub : Hub<IRefreshClient>
{
    private readonly IShowRefresher _showRefresher;
    private readonly IEpisodeRepository _episodeRepository;
    private readonly ICultureParser _cultureParser;
    private readonly IHttpContextAccessor _accessor;
    private readonly LinkGenerator _generator;
    private readonly IMediaDetailsService _mediaDetailsService;

    public RefreshHub(IShowRefresher showRefresher, IEpisodeRepository episodeRepository, ICultureParser cultureParser,
        IHttpContextAccessor accessor, LinkGenerator generator, IMediaDetailsService mediaDetailsService)
    {
        _showRefresher = showRefresher;
        _episodeRepository = episodeRepository;
        _cultureParser = cultureParser;
        _accessor = accessor;
        _generator = generator;
        _mediaDetailsService = mediaDetailsService;
    }

    /// <summary>
    /// Trigger the refresh of a show
    /// </summary>
    /// <param name="showId"></param>
    public async Task RefreshShow(Guid showId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, showId.ToString());
        var show = await _showRefresher.GetShowByGuidAsync(showId, default);
        if (show == null)
        {
            return;
        }

        BackgroundJob.Enqueue<RefreshSingleShowJob>(showJob => showJob.ExecuteAsync(show.Id, null, default));
    }
    

    /// <summary>
    /// Get episode for specific show, season and language
    /// To be used after refresh was successful
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="language"></param>
    /// <param name="season"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async IAsyncEnumerable<EpisodeWithSubtitlesDto> GetEpisodes(Guid showId, string language, int season, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var show = await _showRefresher.GetShowByGuidAsync(showId, default);
        if (show == null)
        {
            yield break;
        }

        var searchLanguage = await _cultureParser.FromStringAsync(language, cancellationToken);

        if (searchLanguage == null)
        {
            yield break;
        }

        var episodes = _episodeRepository.GetSeasonEpisodesByLangUntrackedAsync(show.Id, searchLanguage, season)
            .Select(episode =>
            {
                var subs = episode
                    .Subtitles
                    .Select(
                        subtitle =>
                            new SubtitleDto(subtitle,
                                _generator.GetUriByRouteValues(_accessor.HttpContext!, nameof(Routes.DownloadSubtitle), new { subtitleId = subtitle.UniqueId }) ??
                                throw new InvalidOperationException("Couldn't find the route for the download subtitle"),
                                searchLanguage)
                    );
                return new EpisodeWithSubtitlesDto(episode, subs);
            });
        await foreach (var episode in episodes.WithCancellation(cancellationToken))
        {
            yield return episode;
        }
    }

    /// <summary>
    /// Unsubscribe from getting refresh information about specific show
    /// </summary>
    /// <param name="showId"></param>
    /// <returns></returns>
    public Task UnsubscribeRefreshShow(Guid showId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, showId.ToString());
    }
}