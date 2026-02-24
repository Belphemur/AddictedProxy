using AddictedProxy.Model.Dto;

namespace AddictedProxy.Model.Responses;

public class TvShowSubtitleResponse
{
    /// <summary>
    /// Episode with their subtitles
    /// </summary>
    public IAsyncEnumerable<EpisodeWithSubtitlesDto> Episodes { get; }

    /// <summary>
    /// Season pack subtitles available for this season
    /// </summary>
    public IEnumerable<SeasonPackSubtitleDto> SeasonPacks { get; }

    public TvShowSubtitleResponse(IAsyncEnumerable<EpisodeWithSubtitlesDto> episodes, IEnumerable<SeasonPackSubtitleDto> seasonPacks)
    {
        Episodes = episodes;
        SeasonPacks = seasonPacks;
    }
}