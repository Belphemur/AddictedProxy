using System.ComponentModel.DataAnnotations;

namespace AddictedProxy.Model.Dto;

/// <summary>
/// Represent a media with its details and episodes with subtitles
/// </summary>
/// <param name="Details"></param>
/// <param name="EpisodeWithSubtitles"></param>
/// <param name="LastSeasonNumber"></param>
public record struct MediaDetailsWithEpisodeAndSubtitlesDto(MediaDetailsDto Details, IAsyncEnumerable<EpisodeWithSubtitlesDto> EpisodeWithSubtitles,  int? LastSeasonNumber)
{
    [Required]
    public MediaDetailsDto Details { get; init; } = Details;

    [Required]
    public IAsyncEnumerable<EpisodeWithSubtitlesDto> EpisodeWithSubtitles { get; init; } = EpisodeWithSubtitles;

    
    public int? LastSeasonNumber { get; init; } = LastSeasonNumber;
}