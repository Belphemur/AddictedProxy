using System.ComponentModel.DataAnnotations;

namespace AddictedProxy.Model.Dto;

/// <summary>
/// Represent a media with its details and episodes with subtitles
/// </summary>
/// <param name="Details"></param>
/// <param name="EpisodeWithSubtitles"></param>
/// <param name="LastSeasonNumber"></param>
/// <param name="SeasonPacks"></param>
public record struct MediaDetailsWithEpisodeAndSubtitlesDto(MediaDetailsDto Details, IAsyncEnumerable<EpisodeWithSubtitlesDto> EpisodeWithSubtitles, int? LastSeasonNumber, IEnumerable<SeasonPackSubtitleDto> SeasonPacks)
{
    [Required]
    public MediaDetailsDto Details { get; init; } = Details;

    [Required]
    public IAsyncEnumerable<EpisodeWithSubtitlesDto> EpisodeWithSubtitles { get; init; } = EpisodeWithSubtitles;

    
    public int? LastSeasonNumber { get; init; } = LastSeasonNumber;

    /// <summary>
    /// Season pack subtitles available for the last season
    /// </summary>
    public IEnumerable<SeasonPackSubtitleDto> SeasonPacks { get; init; } = SeasonPacks;
}