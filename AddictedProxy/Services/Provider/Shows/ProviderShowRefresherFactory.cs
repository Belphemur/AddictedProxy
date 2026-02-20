using AddictedProxy.Database.Model.Shows;
using InversionOfControl.Model.Factory;
using Microsoft.Extensions.DependencyInjection;

namespace AddictedProxy.Services.Provider.Shows;

/// <summary>
/// Factory that resolves the correct <see cref="IProviderShowRefresher"/> for a given <see cref="DataSource"/>.
/// </summary>
[EnumServiceLifetime(ServiceLifetime.Scoped)]
public class ProviderShowRefresherFactory : EnumFactory<DataSource, IProviderShowRefresher>
{
    public ProviderShowRefresherFactory(IEnumerable<IProviderShowRefresher> services) : base(services)
    {
    }
}
