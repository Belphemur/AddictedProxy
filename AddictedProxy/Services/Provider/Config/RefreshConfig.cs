namespace AddictedProxy.Services.Provider.Config;

public class RefreshConfig
{
    public class Episode
    {
        public TimeSpan LastSeasonRefresh { get; init; }
        public TimeSpan DefaultRefresh { get; init; }
    }

    public TimeSpan SeasonRefresh { get; init; }
    public Episode EpisodeRefresh { get; init; }

    /// <summary>
    /// How long before a download exceeded cred can be reused
    /// </summary>
    public TimeSpan DownloadExceededTimeout { get; init; }
}