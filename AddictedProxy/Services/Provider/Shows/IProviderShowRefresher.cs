using AddictedProxy.Database.Model.Shows;
using InversionOfControl.Model.Factory;

namespace AddictedProxy.Services.Provider.Shows;

/// <summary>
/// Provider-specific show refresh logic, keyed by <see cref="DataSource"/>.
/// Each provider implements this to handle its own show refresh mechanism.
/// </summary>
public interface IProviderShowRefresher : IEnumService<DataSource>
{
    /// <summary>
    /// Refresh seasons and episodes for a show from this provider.
    /// </summary>
    /// <param name="show">The show to refresh.</param>
    /// <param name="externalId">The provider-specific external ID for the show.</param>
    /// <param name="token">Cancellation token.</param>
    Task RefreshShowAsync(TvShow show, ShowExternalId externalId, CancellationToken token);

    /// <summary>
    /// Whether the show needs to be refreshed from this provider.
    /// </summary>
    bool IsShowNeedsRefresh(TvShow show);
}
