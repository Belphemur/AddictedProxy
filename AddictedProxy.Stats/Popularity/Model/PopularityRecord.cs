using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Stats.Popularity.Model;

public record PopularityCount(long Count, DateTime LastRequested);

public record PopularityRecord(TvShow Show, ImmutableDictionary<CultureInfo, PopularityCount> Counts, long Total);