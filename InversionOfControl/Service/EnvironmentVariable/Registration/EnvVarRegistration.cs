using InversionOfControl.Service.Parser;
using Microsoft.Extensions.DependencyInjection;

namespace InversionOfControl.Service.Registration;

/// <summary>
/// Registration of a EnvironmentVariable
/// </summary>
/// <param name="Key">Unique Key</param>
/// <param name="Lifetime">Lifetime of the EnvironmentVariable</param>
/// <typeparam name="TType">Returned type when parsing the EnvironmentVariable</typeparam>
/// <typeparam name="TParser">Parser to take care of the transformation of the string into <see cref="TType"/></typeparam>
public record EnvVarRegistration<TType, TParser>(string Key, ServiceLifetime Lifetime = ServiceLifetime.Singleton) where TParser : IEnvVarParser<TType> where TType : class
{
    public virtual bool Equals(EnvVarRegistration<TType, TParser>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Key == other.Key;
    }

    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }
}