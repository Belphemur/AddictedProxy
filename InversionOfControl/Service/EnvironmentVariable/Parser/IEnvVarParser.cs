namespace InversionOfControl.Service.Parser;

public interface IEnvVarParser<TType> where TType : class
{
    /// <summary>
    /// Parse an environment variable into the wanted type
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public TType Parse(string value);
}