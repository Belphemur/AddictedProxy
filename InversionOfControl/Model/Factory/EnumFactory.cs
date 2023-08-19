namespace InversionOfControl.Model.Factory;

public class EnumFactory<TEnum, TEnumService>
    where TEnumService : IEnumService<TEnum>
{
    protected readonly Dictionary<TEnum, TEnumService> Services;
    public EnumFactory(IEnumerable<TEnumService> services)
    {
        Services = services.ToDictionary(service => service.Enum);
    }
    
    /// <summary>
    /// Get the service for the given key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">If the key isn't present in the factory</exception>
    public TEnumService GetService(TEnum key)
    {
        if (!Services.TryGetValue(key, out var service))
        {
            throw new ArgumentOutOfRangeException(nameof(key), key, $"No service ({typeof(TEnumService).Name}) found for the given key: {key}");
        }

        return service;
    }
}