using AddictedProxy.Database.Model.Shows;
using InversionOfControl.Model.Factory;

namespace AddictedProxy.Services.Provider.Seasons;

/// <summary>
/// Provider-specific season refresh logic, keyed by <see cref="DataSource"/>.
/// Each provider implements this to handle its own season refresh mechanism.
/// </summary>
public interface IProviderSeasonRefresher : IEnumService<DataSource>
{
    /// <summary>
    /// Get a specific season, refreshing from the provider if needed.
    /// </summary>
    /// <param name="show">The show.</param>
    /// <param name="externalId">The provider-specific external ID for the show.</param>
    /// <param name="seasonNumber">The season number to find.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>The season if found after refresh, null otherwise.</returns>
    Task<Season?> GetRefreshSeasonAsync(TvShow show, ShowExternalId externalId, int seasonNumber, CancellationToken token);

    /// <summary>
    /// Refresh all seasons for a show from this provider.
    /// </summary>
    /// <param name="show">The show to refresh seasons for.</param>
    /// <param name="externalId">The provider-specific external ID for the show.</param>
    /// <param name="token">Cancellation token.</param>
    Task RefreshSeasonsAsync(TvShow show, ShowExternalId externalId, CancellationToken token);

    /// <summary>
    /// Whether the show needs to have its seasons refreshed from this provider.
    /// </summary>
    bool IsShowNeedsRefresh(TvShow show);
}
