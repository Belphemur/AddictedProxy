using InversionOfControl.Service.EnvironmentVariable.Parser;
using InversionOfControl.Service.EnvironmentVariable.Registration;

namespace InversionOfControl.Model;

public interface IBootstrapEnvironmentVariable<TType, TParser> where TParser : IEnvVarParser<TType> where TType : class
{
    /// <summary>
    /// Registration of the Environment Variable
    /// </summary>
    public EnvVarRegistration<TType, TParser> EnvVarRegistration { get; }
}