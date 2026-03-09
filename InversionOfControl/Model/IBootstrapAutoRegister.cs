namespace InversionOfControl.Model;

/// <summary>
/// Marker interface for automatic DI registration of all implementations of <typeparamref name="TInterface"/>
/// found in the same assembly.
/// <para>
/// Implement this interface on a <c>partial</c> class that also implements <see cref="IBootstrap"/>.
/// A source generator will produce the <see cref="IBootstrap.ConfigureServices"/> method body,
/// registering every concrete class that implements <typeparamref name="TInterface"/>.
/// </para>
/// <para>
/// The service lifetime defaults to <see cref="Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped"/>.
/// Apply <see cref="Factory.ServiceLifetimeAttribute"/> to the class to override the lifetime.
/// </para>
/// </summary>
/// <typeparam name="TInterface">The service interface whose implementations should be auto-registered.</typeparam>
public interface IBootstrapAutoRegister<TInterface>;
