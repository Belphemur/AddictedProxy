using AddictedProxy.Model.Dto;

namespace AddictedProxy.Model.Responses;

public class SubtitleSearchResponse
{
    public SubtitleSearchResponse(IEnumerable<SubtitleDto> matchingSubtitles, EpisodeDto episode)
    {
        MatchingSubtitles = matchingSubtitles;
        Episode = episode;
    }

    /// <summary>
    /// Matching subtitle for the filename and language
    /// </summary>
    public IEnumerable<SubtitleDto> MatchingSubtitles { get; }

    /// <summary>
    /// Information about the episode
    /// </summary>
    public EpisodeDto Episode { get; }
}