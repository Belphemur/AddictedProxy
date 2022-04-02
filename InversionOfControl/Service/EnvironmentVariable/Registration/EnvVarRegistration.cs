using InversionOfControl.Service.EnvironmentVariable.Parser;
using Microsoft.Extensions.DependencyInjection;

namespace InversionOfControl.Service.EnvironmentVariable.Registration;

/// <summary>
///     Registration of a EnvironmentVariable
/// </summary>
/// <param name="Keys">Unique Keys for this registration</param>
/// <param name="Lifetime">Lifetime of the EnvironmentVariable</param>
/// <typeparam name="TType">Returned type when parsing the EnvironmentVariable</typeparam>
/// <typeparam name="TParser">Parser to take care of the transformation of the string into <see cref="TType" /></typeparam>
public record EnvVarRegistration<TType, TParser>(ServiceLifetime Lifetime, params string[] Keys) where TParser : IEnvVarParser<TType> where TType : class
{
    public EnvVarRegistration(params string[] keys) : this(ServiceLifetime.Singleton, keys)
    {
    }
}