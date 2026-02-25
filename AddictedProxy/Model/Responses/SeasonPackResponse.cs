using AddictedProxy.Model.Dto;

namespace AddictedProxy.Model.Responses;

/// <summary>
/// Response containing season pack subtitles for a given season and language
/// </summary>
/// <param name="SeasonPacks">Season pack subtitles</param>
public record SeasonPackResponse(IEnumerable<SeasonPackSubtitleDto> SeasonPacks);
