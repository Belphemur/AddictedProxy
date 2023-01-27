#region

using Microsoft.AspNetCore.Builder;

#endregion

namespace InversionOfControl.Model;

public interface IBootstrapApp
{
    /// <summary>
    ///     Configure the different application extensions
    /// </summary>
    public void ConfigureApp(IApplicationBuilder app);
}