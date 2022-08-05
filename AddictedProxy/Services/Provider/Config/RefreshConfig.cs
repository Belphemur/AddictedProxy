namespace AddictedProxy.Services.Provider.Config;

public class RefreshConfig
{
    public class Episode
    {
        /// <summary>
        /// How often do we refresh the last season of a non completed show
        /// </summary>
        public TimeSpan LastSeasonRefresh { get; init; }
        /// <summary>
        /// Default refresh time out
        /// </summary>
        public TimeSpan DefaultRefresh { get; init; }
        
        /// <summary>
        /// When a show is completed, how long before trying to fetch episodes
        /// </summary>
        public TimeSpan CompletedShowRefresh { get; init; }
    }

    public TimeSpan SeasonRefresh { get; init; }
    public Episode EpisodeRefresh { get; init; }

    /// <summary>
    /// How long before a download exceeded cred can be reused
    /// </summary>
    public TimeSpan DownloadExceededTimeout { get; init; }
}