namespace AddictedProxy.Model.Dto;

public record struct MediaDetailsWithEpisodeAndSubtitlesDto(MediaDetailsDto Details, IAsyncEnumerable<EpisodeWithSubtitlesDto> EpisodeWithSubtitles, int? LastSeasonNumber);