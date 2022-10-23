using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Model.Search;

public record SearchPayload(TvShow Show, int Episode, int Season, string LanguageIso, string? FileName);