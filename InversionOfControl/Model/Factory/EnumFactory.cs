namespace InversionOfControl.Model.Factory;

public class EnumFactory<TEnum, TEnumService>
    where TEnumService : IEnumService<TEnum>
{
    protected readonly Dictionary<TEnum, TEnumService> ServicesByEnum;

    public EnumFactory(IEnumerable<TEnumService> services)
    {
        ServicesByEnum = services.ToDictionary(service => service.Enum);
    }

    /// <summary>
    /// Keys of the factory
    /// </summary>
    public IReadOnlyCollection<TEnum> Keys => ServicesByEnum.Keys;

    /// <summary>
    /// All the services of the factory
    /// </summary>
    public IReadOnlyCollection<TEnumService> Services => ServicesByEnum.Values;
    /// <summary>
    /// Get the service for the given key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">If the key isn't present in the factory</exception>
    public TEnumService GetService(TEnum key)
    {
        if (!ServicesByEnum.TryGetValue(key, out var service))
        {
            throw new ArgumentOutOfRangeException(nameof(key), key, $"No service ({typeof(TEnumService).Name}) found for the given key: {key}");
        }

        return service;
    }
}