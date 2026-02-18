using AddictedProxy.Database.Model.Shows;
using InversionOfControl.Model.Factory;
using Microsoft.Extensions.DependencyInjection;

namespace AddictedProxy.Services.Provider.Seasons;

/// <summary>
/// Factory that resolves the correct <see cref="IProviderSeasonRefresher"/> for a given <see cref="DataSource"/>.
/// </summary>
[EnumServiceLifetime(ServiceLifetime.Scoped)]
public class ProviderSeasonRefresherFactory : EnumFactory<DataSource, IProviderSeasonRefresher>
{
    public ProviderSeasonRefresherFactory(IEnumerable<IProviderSeasonRefresher> services) : base(services)
    {
    }
}
