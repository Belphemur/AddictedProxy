namespace InversionOfControl.Model.Factory;

public class EnumFactory<TEnum, TEnumService>
    where TEnum : IEquatable<TEnum>, IComparable<TEnum>
    where TEnumService : IEnumService<TEnum>
{
    private readonly Dictionary<TEnum, TEnumService> _services;
    public EnumFactory(IEnumerable<TEnumService> services)
    {
        _services = services.ToDictionary(service => service.Enum);
    }
    
    /// <summary>
    /// Get the service for the given key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public TEnumService GetService(TEnum key)
    {
        return _services[key];
    }
}