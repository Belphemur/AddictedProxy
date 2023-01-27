using AddictedProxy.Database.Model.Shows;

namespace AddictedProxy.Stats.Popularity.Model;

public record DownloadPopularity(TvShow Show, long TotalDownloads);