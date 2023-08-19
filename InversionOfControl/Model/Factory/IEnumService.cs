namespace InversionOfControl.Model.Factory;

/// <summary>
/// Service provided by a factory
/// </summary>
/// <typeparam name="TEnum"></typeparam>
public interface IEnumService<out TEnum>
{
    public TEnum Enum { get; }
}