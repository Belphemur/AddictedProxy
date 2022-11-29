using System.Globalization;
using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Model.Dto;

namespace AddictedProxy.Model.Search;

public record SubtitleFound(IEnumerable<Subtitle> MatchingSubtitles, EpisodeDto Episode, Culture.Model.Culture Language);