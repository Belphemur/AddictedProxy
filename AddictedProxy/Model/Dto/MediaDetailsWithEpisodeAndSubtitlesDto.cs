namespace AddictedProxy.Model.Dto;

public record MediaDetailsWithEpisodeAndSubtitlesDto(MediaDetailsDto Details, EpisodeWithSubtitlesDto[] EpisodeWithSubtitles, int? LastSeasonNumber);