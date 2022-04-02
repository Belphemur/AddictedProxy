using InversionOfControl.Service.EnvironmentVariable.Registration;

namespace InversionOfControl.Service.EnvironmentVariable.Parser;

public interface IEnvVarParser<TType> where TType : class
{
    /// <summary>
    /// Parse an environment variable into the wanted type
    /// </summary>
    /// <param name="keys">Environment Variable names received</param>
    /// <param name="values">Dictionary of the value of the keys</param>
    /// <returns></returns>
    public TType Parse(string[] keys, Dictionary<string, string> values);
}