using InversionOfControl.Model.Factory;
using Microsoft.Extensions.DependencyInjection;

namespace InversionOfControl.Tests.Mock.Factory;

public enum FactType3
{
    Alpha,
    Beta
}

/// <summary>
/// A scoped dependency to prove scoped services can be consumed from a scoped factory.
/// </summary>
public interface IScopedDependency
{
    string Value { get; }
}

public class ScopedDependency : IScopedDependency
{
    public string Value => "scoped";
}

public interface IFactoryServiceMock3 : IEnumService<FactType3>
{
}

public class AlphaService : IFactoryServiceMock3
{
    public FactType3 Enum => FactType3.Alpha;
}

public class BetaService : IFactoryServiceMock3
{
    public FactType3 Enum => FactType3.Beta;
}

/// <summary>
/// Concrete factory subclass with <see cref="EnumServiceLifetimeAttribute"/> overriding to Scoped.
/// </summary>
[EnumServiceLifetime(ServiceLifetime.Scoped)]
public class ScopedFactoryMock : EnumFactory<FactType3, IFactoryServiceMock3>
{
    public ScopedFactoryMock(IEnumerable<IFactoryServiceMock3> services) : base(services)
    {
    }
}
