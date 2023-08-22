namespace InversionOfControl.Model.Factory;

public class EnumFactory<TEnum, TEnumService>
    where TEnumService : IEnumService<TEnum>
{
    private readonly Dictionary<TEnum, TEnumService> _servicesByEnum;

    public EnumFactory(IEnumerable<TEnumService> services)
    {
        _servicesByEnum = services.ToDictionary(service => service.Enum);
    }

    /// <summary>
    /// Keys of the factory
    /// </summary>
    public IReadOnlyCollection<TEnum> Keys => _servicesByEnum.Keys;

    /// <summary>
    /// All the services of the factory
    /// </summary>
    public IReadOnlyCollection<TEnumService> Services => _servicesByEnum.Values;
    /// <summary>
    /// Get the service for the given key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">If the key isn't present in the factory</exception>
    public TEnumService GetService(TEnum key)
    {
        if (!_servicesByEnum.TryGetValue(key, out var service))
        {
            throw new ArgumentOutOfRangeException(nameof(key), key, $"No service ({typeof(TEnumService).Name}) found for the given key: {key}");
        }

        return service;
    }
}