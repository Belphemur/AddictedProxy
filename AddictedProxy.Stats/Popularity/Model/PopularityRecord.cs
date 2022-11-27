using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Stats.Popularity.Model;

public record DownloadPopularity(TvShow Show, long TotalDownloads);