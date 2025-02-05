using System.Reflection;
using InversionOfControl.Extensions;
using InversionOfControl.Model;
using InversionOfControl.Model.Factory;
using InversionOfControl.Service.EnvironmentVariable.Exception;
using InversionOfControl.Service.EnvironmentVariable.Parser;
using InversionOfControl.Service.EnvironmentVariable.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace InversionOfControl.Service.Bootstrap;

internal class BootstrapRegister : IDisposable
{
    private Dictionary<Assembly, Type[]>? _assemblyTypeCache = [];
    private readonly Type _bootstrapType = typeof(IBootstrap);
    private readonly Type _bootstrapConditionalType = typeof(IBootstrapConditional);
    private readonly Type _bootstrapEnv = typeof(IBootstrapEnvironmentVariable<,>);
    private readonly Type _bootstrapApp = typeof(IBootstrapApp);
    private readonly Type _envVarRegistrationType = typeof(EnvVarRegistration<,>);
    private readonly Type _factoryServiceType = typeof(IEnumService<>);
    private readonly Type _factoryType = typeof(EnumFactory<,>);

    /// <summary>
    /// Register all the different services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="assemblies"></param>
    /// <exception cref="EnvironmentVariableException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public void RegisterBootstrapServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging, params Assembly[] assemblies)
    {
        var keys = new Dictionary<string, Type>();
        var factories = new HashSet<Type>();

        void RegisterEnvVar(Type[] genericTypes, object registration, Type currentBootstrapType)
        {
            var envVarType = genericTypes[0];
            var parserType = genericTypes[1];

            var currentEnvVarRegistrationType = _envVarRegistrationType.MakeGenericType(genericTypes);
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
                return parserType.GetMethod(nameof(VoidParser.Parse))!.Invoke(parser, [currentKeys, keyValues]);
            }, lifeTime));
        }

        void RegisterFactory(Type type, Type serviceInterface)
        {
            var interfaceInheritingFromServiceInterface = type.FindInterfaces((interfaceType, _) => interfaceType.GetInterface(_factoryServiceType.FullName) != null, null).SingleOrDefault();
            if (interfaceInheritingFromServiceInterface == null)
            {
                throw new ArgumentNullException($"{type} isn't implementing an interface of the factory pattern ({_factoryServiceType.Name}).");
            }

            var enumType = serviceInterface.GetGenericArguments().SingleOrDefault();
            if (enumType == null)
            {
                throw new ArgumentNullException($"{type} isn't implementing an interface of the factory pattern ({_factoryServiceType.Name}).");
            }
            services.Add(new ServiceDescriptor(interfaceInheritingFromServiceInterface, type, ServiceLifetime.Singleton));

            var enumFactoryType = _factoryType.MakeGenericType(enumType, interfaceInheritingFromServiceInterface);

            if (!factories.Contains(enumFactoryType))
            {
                services.Add(new ServiceDescriptor(enumFactoryType, enumFactoryType, ServiceLifetime.Singleton));
                factories.Add(enumFactoryType);
            }
        }

        foreach (var assembly in assemblies)
        {
            var types = GetSetCachedTypes(assembly);

            foreach (var type in types)
            {
                //Handle registration of factories
                var serviceInterface = type.GetInterface(_factoryServiceType.FullName!);
                if (serviceInterface != null)
                {
                    RegisterFactory(type, serviceInterface);
                    continue;
                }

                object? bootstrap = null;
                if (_bootstrapConditionalType.IsAssignableFrom(type))
                {
                    bootstrap ??= Activator.CreateInstance(type);
                    if (!((IBootstrapConditional)bootstrap).ShouldLoadBootstrap(configuration))
                    {
                        Console.Out.WriteLine($"[Bootstrap] Condition not met to load boostrap: {type}");
                        continue;
                    }

                    Console.Out.WriteLine($"[Bootstrap] Condition met, loading boostrap: {type}");
                }

                if (_bootstrapType.IsAssignableFrom(type))
                {
                    bootstrap ??= Activator.CreateInstance(type);
                    ((IBootstrap)bootstrap).ConfigureServices(services, configuration, logging);
                    foreach (var dependency in ((IBootstrap)bootstrap).Dependencies)
                    {
                        RegisterBootstrapServices(services, configuration, logging, dependency);
                    }
                }

                if (_bootstrapEnv.IsAssignableToGenericType(type))
                {
                    bootstrap ??= Activator.CreateInstance(type);
                    foreach (var interfaceBootstrapEnvVarType in type.GetInterfaces().Where(_bootstrapEnv.IsAssignableToGenericType))
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
            }
        }

        Validate(keys.Keys);
    }

    /// <summary>
    /// Register the bootstrap for the application
    /// </summary>
    /// <param name="application"></param>
    /// <param name="assemblies"></param>
    public void RegisterBootstrapApp(IApplicationBuilder application, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var types = GetSetCachedTypes(assembly);

            foreach (var type in types)
            {
                object? bootstrap;
                if (_bootstrapApp.IsAssignableFrom(type))
                {
                    bootstrap = Activator.CreateInstance(type);
                    ((IBootstrapApp)bootstrap).ConfigureApp(application);
                }
            }
        }
    }

    private Type[] GetSetCachedTypes(Assembly assembly)
    {
        if (_assemblyTypeCache!.TryGetValue(assembly, out var types))
        {
            return types;
        }

        types = assembly.GetTypes().Where(type => !type.IsInterface).ToArray();
        _assemblyTypeCache[assembly] = types;

        return types;
    }

    private void Validate(IEnumerable<string> keys)
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

    public void Dispose()
    {
        _assemblyTypeCache?.Clear();
        _assemblyTypeCache = null;
        GC.SuppressFinalize(this);
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