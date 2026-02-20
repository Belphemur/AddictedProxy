using Microsoft.Extensions.DependencyInjection;

namespace InversionOfControl.Model.Factory;

/// <summary>
/// Override the DI service lifetime for an <see cref="EnumFactory{TEnum,TEnumService}"/> and its services.
/// Place on a concrete factory subclass to change the registration lifetime from the default <see cref="ServiceLifetime.Singleton"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class EnumServiceLifetimeAttribute(ServiceLifetime lifetime) : Attribute
{
    public ServiceLifetime Lifetime { get; } = lifetime;
}
