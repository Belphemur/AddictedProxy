using System;
using System.Globalization;
using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Stats.Popularity.Model;

/// <summary>
/// 
/// </summary>
/// <param name="TvShowId"></param>
/// <param name="Language"></param>
/// <param name="Requested">If null, will use DateTime.UtcNow</param>
public record RecordPopularityPayload(long TvShowId, CultureInfo Language, DateTime? Requested = null);