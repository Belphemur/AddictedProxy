using AddictedProxy.Database.Model.Shows;
using InversionOfControl.Model.Factory;

namespace AddictedProxy.Services.Provider.Shows;

/// <summary>
/// Factory that resolves the correct <see cref="IProviderShowRefresher"/> for a given <see cref="DataSource"/>.
/// </summary>
public class ProviderShowRefresherFactory : EnumFactory<DataSource, IProviderShowRefresher>
{
    public ProviderShowRefresherFactory(IEnumerable<IProviderShowRefresher> services) : base(services)
    {
    }
}
