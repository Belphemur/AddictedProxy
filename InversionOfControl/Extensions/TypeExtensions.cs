namespace InversionOfControl.Extensions;

public static class TypeExtensions
{
    /// <summary>
    ///     Check that the generic type is assignable
    /// </summary>
    /// <param name="genericType"></param>
    /// <param name="givenType"></param>
    /// <returns></returns>
    public static bool IsAssignableToGenericType(this Type genericType, Type givenType)
    {
        var interfaceTypes = givenType.GetInterfaces();

        if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
        {
            return true;
        }

        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
        {
            return true;
        }

        var baseType = givenType.BaseType;
        return baseType != null && IsAssignableToGenericType(baseType, genericType);
    }
}