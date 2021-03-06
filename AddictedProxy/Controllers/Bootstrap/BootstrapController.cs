#region

using AddictedProxy.Controllers.Middleware;
using AddictedProxy.Controllers.Rest.Serializer;
using AddictedProxy.Services.Middleware;
using InversionOfControl.Model;
using Microsoft.Net.Http.Headers;

#endregion

namespace AddictedProxy.Controllers.Rest.Bootstrap;

public class BootstrapController : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddMvcOptions(options => options.Filters.Add<OperationCancelledExceptionFilter>())
            .AddJsonOptions(options => options.JsonSerializerOptions.AddContext<SerializationContext>());
        services.AddLogging(opt => { opt.AddConsole(c => { c.TimestampFormat = "[HH:mm:ss] "; }); });
        services.AddResponseCaching();
        services.AddSingleton<DefaultResponseCachingMiddleware>();
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        if (app is IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllers();
        }
        
        app.UseCors(policyBuilder =>
        {
            policyBuilder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("Content-Disposition");
            if (app.ApplicationServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                policyBuilder.SetIsOriginAllowed(_ => true);
            }
            else
            {
                policyBuilder.SetIsOriginAllowed(hostname => hostname.EndsWith(".gestdown.info"));
            }
        });

        app.UseHttpLogging();
        app.UseResponseCaching();
        app.UseMiddleware<DefaultResponseCachingMiddleware>();
        app.UseRouting();
        app.UseAuthorization();
        
    }
}