using InversionOfControl.Exception;
using InversionOfControl.Model;
using InversionOfControl.Service.Parser;
using InversionOfControl.Service.Registration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace InversionOfControl.Service;

public static class BootstrapRegistrarExtensions
{
    /// <summary>
    /// Add the different <see cref="IBootstrap"/> registration to the IoC container
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddBootstrap(this IServiceCollection services)
    {
        var bootstrapType = typeof(IBootstrap);
        foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                                      .SelectMany(s => s.GetTypes())
                                      .Where(p => p.IsClass)
                                      .Where(p => bootstrapType.IsAssignableFrom(p)))
        {
            var bootstrap = (IBootstrap)type.GetConstructor(Type.EmptyTypes)!.Invoke(Array.Empty<object>());
            bootstrap.ConfigureServices(services);
        }

        return services;
    }

    public static IServiceCollection AddBootstrapEnvironmentVar(this ServiceCollection services)
    {
        var bootstrapType = typeof(IBootstrapEnvironmentVariable<,>);
        var envVarRegistrationType = typeof(EnvVarRegistration<,>);
        var parserGenericType = typeof(IEnvVarParser<>);
        var keys = new Dictionary<string, Type>();
        foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                                      .SelectMany(s => s.GetTypes())
                                      .Where(p => p.IsClass)
                                      .Where(p => bootstrapType.IsAssignableFrom(p)))
        {
            var bootstrap = type.GetConstructor(Type.EmptyTypes)!.Invoke(Array.Empty<object>());
            var registration = type.GetProperty("EnvVarRegistration")!.GetValue(bootstrap);
            if (registration == null)
            {
                throw new ArgumentNullException($"If you use the {typeof(IBootstrapEnvironmentVariable<,>)}, you need to set the env var registration.");
            }

            var genericArguments = type.GetGenericArguments();
            var envVarType = genericArguments[0];
            var parserType = genericArguments[1];
            if (!parserGenericType.IsAssignableFrom(parserType))
            {
                throw new EnvironmentVariableException("N/A", $"The parser given for {type} don't implement the right interface {parserType}");
            }
            var currentEnvVarRegistrationType = envVarRegistrationType.MakeGenericType(genericArguments);
            var key = (string)currentEnvVarRegistrationType.GetProperty("Key")!.GetValue(registration);
            var lifeTime = (ServiceLifetime)currentEnvVarRegistrationType.GetProperty("LifeTime")!.GetValue(registration);
            if (keys.TryGetValue(key, out var alreadyRegisteredType))
            {
                throw new EnvironmentVariableException(key, $"{key} is already registered by {alreadyRegisteredType.Name}.");
            }
            keys.Add(key, type);
            
            services.TryAddSingleton(parserType);
            services.TryAdd(new ServiceDescriptor(envVarType, factory =>
            {
                var parser = factory.GetRequiredService(parserType);
                return parserType.GetMethod("Parse")!.Invoke(parser, new object[] { Environment.GetEnvironmentVariable(key) });

            }, lifeTime));
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
                throw new AggregateException($"Multiple environment vars missing: {string.Join(", ", errors.Select(exception => exception.Key))}", errors.Cast<System.Exception>());
            case 1:
                throw errors[0];
        }
    }
}