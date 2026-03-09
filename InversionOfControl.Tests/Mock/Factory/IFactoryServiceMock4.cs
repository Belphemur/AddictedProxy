using InversionOfControl.Model.Factory;
using Microsoft.Extensions.DependencyInjection;

namespace InversionOfControl.Tests.Mock.Factory;

public enum FactType4
{
    Gamma,
    Delta
}

public interface IFactoryServiceMock4 : IEnumService<FactType4>
{
}

public class GammaService : IFactoryServiceMock4
{
    public FactType4 Enum => FactType4.Gamma;
}

public class DeltaService : IFactoryServiceMock4
{
    public FactType4 Enum => FactType4.Delta;
}

/// <summary>
/// Concrete factory subclass with <see cref="ServiceLifetimeAttribute"/> overriding to Transient.
/// </summary>
[ServiceLifetime(ServiceLifetime.Transient)]
public class TransientFactoryMock : EnumFactory<FactType4, IFactoryServiceMock4>
{
    public TransientFactoryMock(IEnumerable<IFactoryServiceMock4> services) : base(services)
    {
    }
}
