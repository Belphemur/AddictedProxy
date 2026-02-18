using AddictedProxy.Database.Model.Shows;
using InversionOfControl.Model.Factory;

namespace AddictedProxy.Services.Provider.Episodes;

/// <summary>
/// Factory that resolves the correct <see cref="IProviderEpisodeRefresher"/> for a given <see cref="DataSource"/>.
/// </summary>
public class ProviderEpisodeRefresherFactory : EnumFactory<DataSource, IProviderEpisodeRefresher>
{
    public ProviderEpisodeRefresherFactory(IEnumerable<IProviderEpisodeRefresher> services) : base(services)
    {
    }
}
