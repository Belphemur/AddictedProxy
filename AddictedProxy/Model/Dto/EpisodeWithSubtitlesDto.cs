using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Model.Dto;

public class EpisodeWithSubtitlesDto : EpisodeDto
{
    /// <summary>
    /// Subtitles for this episode
    /// </summary>
    public IEnumerable<SubtitleDto> Subtitles { get; }

    public EpisodeWithSubtitlesDto(Episode episode, IEnumerable<SubtitleDto> subtitles) : base(episode)
    {
        Subtitles = subtitles;
    }
}