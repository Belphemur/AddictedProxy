using AddictedProxy.Model.Dto;

namespace AddictedProxy.Model.Responses;

public class TvShowSubtitleResponse
{
    /// <summary>
    /// Episode with their subtitles
    /// </summary>
    public IAsyncEnumerable<EpisodeWithSubtitlesDto> Episodes { get; }

    public TvShowSubtitleResponse(IAsyncEnumerable<EpisodeWithSubtitlesDto> episodes)
    {
        Episodes = episodes;
    }
}