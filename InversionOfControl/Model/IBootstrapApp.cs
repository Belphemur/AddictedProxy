#region

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace InversionOfControl.Model;

public interface IBootstrapApp
{
    /// <summary>
    ///     Configure the different application extensions
    /// </summary>
    public void ConfigureApp(IApplicationBuilder application);
}