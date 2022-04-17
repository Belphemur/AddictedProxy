#region

using System.Reflection;
using InversionOfControl.Extensions;
using InversionOfControl.Model;
using InversionOfControl.Service.EnvironmentVariable.Exception;
using InversionOfControl.Service.EnvironmentVariable.Parser;
using InversionOfControl.Service.EnvironmentVariable.Registration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

#endregion

namespace InversionOfControl.Service.Bootstrap;

public static class BootstrapRegistrarExtensions
{
    /// <summary>
    ///     Add the different <see cref="IBootstrap" /> registration to the IoC container
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assemblies">Where to look for <see cref="IBootstrap" /></param>
    /// <returns></returns>
    public static IServiceCollection AddBootstrap(this IServiceCollection services, params Assembly[] assemblies)
    {
        var bootstrapType = typeof(IBootstrap);
        if (assemblies.Length == 0)
        {
            throw new ArgumentException($"Need minimum one assembly to register {bootstrapType}");
        }

        foreach (var type in assemblies
                             .SelectMany(s => s.GetTypes())
                             .Where(p => p.IsClass)
                             .Where(p => bootstrapType.IsAssignableFrom(p)))
        {
            var bootstrap = (IBootstrap)Activator.CreateInstance(type);
            bootstrap.ConfigureServices(services);
        }

        return services;
    }

    /// <summary>
    ///     Register the different environment variables
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assemblies">Where to look for <see cref="IBootstrapEnvironmentVariable{TType,TParser}" /></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="EnvironmentVariableException"></exception>
    public static IServiceCollection AddBootstrapEnvironmentVar(this IServiceCollection services, params Assembly[] assemblies)
    {
        var bootstrapType = typeof(IBootstrapEnvironmentVariable<,>);

        if (assemblies.Length == 0)
        {
            throw new ArgumentException($"Need minimum one assembly to register {bootstrapType}");
        }

        var envVarRegistrationType = typeof(EnvVarRegistration<,>);
        var keys = new Dictionary<string, Type>();

        void RegisterEnvVar(Type[] genericTypes, object registration, Type currentBootstrapType)
        {
            var envVarType = genericTypes[0];
            var parserType = genericTypes[1];

            var currentEnvVarRegistrationType = envVarRegistrationType.MakeGenericType(genericTypes);
            var currentKeys = (string[])currentEnvVarRegistrationType.GetProperty(nameof(EnvVarRegistration<Void, VoidParser>.Keys))!.GetValue(registration);
            var lifeTime = (ServiceLifetime)currentEnvVarRegistrationType.GetProperty(nameof(EnvVarRegistration<Void, VoidParser>.Lifetime))!.GetValue(registration);
            foreach (var key in currentKeys)
            {
                if (keys.TryGetValue(key, out var alreadyRegisteredType))
                {
                    throw new EnvironmentVariableException(key, $"{key} is already registered by {alreadyRegisteredType.Name}.");
                }

                keys.Add(key, currentBootstrapType);
            }

            services.TryAddSingleton(parserType);
            services.TryAdd(new ServiceDescriptor(envVarType, factory =>
            {
                var parser = factory.GetRequiredService(parserType);
                var keyValues = currentKeys.ToDictionary(s => s, Environment.GetEnvironmentVariable);
                return parserType.GetMethod(nameof(VoidParser.Parse))!.Invoke(parser, new object[] { currentKeys, keyValues });
            }, lifeTime));
        }

        foreach (var type in assemblies
                             .SelectMany(s => s.GetTypes())
                             .Where(p => !p.IsInterface)
                             .Where(bootstrapType.IsAssignableToGenericType))
        {
            var bootstrap = Activator.CreateInstance(type);

            foreach (var interfaceBootstrapEnvVarType in type.GetInterfaces().Where(bootstrapType.IsAssignableToGenericType))
            {
                var genericArguments = interfaceBootstrapEnvVarType.GetGenericArguments();
                var registration = interfaceBootstrapEnvVarType.GetProperty(nameof(VoidBootstrap.EnvVarRegistration))!.GetValue(bootstrap);
                if (registration == null)
                {
                    throw new ArgumentNullException($"If you use the {typeof(IBootstrapEnvironmentVariable<,>)}, you need to set the env var registration.");
                }

                RegisterEnvVar(genericArguments, registration, type);
            }
        }

        Validate(keys.Keys);

        return services;
    }

    private static void Validate(IEnumerable<string> keys)
    {
        var errors = keys
                     .Where(key => Environment.GetEnvironmentVariable(key) == null)
                     .Select(key => new EnvironmentVariableException(key, $"{key} couldn't be found in the environment vars."))
                     .ToArray();

        switch (errors.Length)
        {
            case > 1:
                throw new AggregateException($"Multiple environment vars missing: {string.Join(", ", errors.Select(exception => exception.Key))}", errors.Cast<Exception>());
            case 1:
                throw errors[0];
        }
    }

    #region MockupBootstrapEnv

    private record Void;

    private class VoidParser : IEnvVarParser<Void>
    {
        public Void Parse(string[] keys, Dictionary<string, string> values)
        {
            throw new NotImplementedException();
        }
    }

    private class VoidBootstrap : IBootstrapEnvironmentVariable<Void, VoidParser>
    {
        public EnvVarRegistration<Void, VoidParser> EnvVarRegistration { get; }
    }

    #endregion
}