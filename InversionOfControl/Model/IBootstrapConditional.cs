using Microsoft.Extensions.Configuration;

namespace InversionOfControl.Model;

public interface IBootstrapConditional : IBootstrap
{
    /// <summary>
    /// Should this boostrap be loaded
    /// </summary>
    /// <returns></returns>
    public bool ShouldLoadBootstrap(IConfiguration configuration);
}