#region

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;
using AddictedProxy.Controllers.Rest.Serializer;
using AddictedProxy.Services.Middleware;
using InversionOfControl.Model;
using Prometheus;

#endregion

namespace AddictedProxy.Controllers.Bootstrap;

public partial class BootstrapController : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
                .AddMvcOptions(options => options.Filters.Add<OperationCancelledExceptionFilter>())
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine(SerializationContext.Default, new DefaultJsonTypeInfoResolver());
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
        services.AddHttpContextAccessor();
        services.AddHttpLogging(_ => { });
        services.AddLogging(opt => { opt.AddConsole(c => { c.TimestampFormat = "[HH:mm:ss] "; }); });
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
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .WithExposedHeaders("Content-Disposition")
                .WithExposedHeaders("sentry-trace")
                .WithExposedHeaders("baggage");
            if (app.ApplicationServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                policyBuilder.SetIsOriginAllowed(_ => true);
            }
            else
            {
                policyBuilder.SetIsOriginAllowed(hostname => CorsOriginRegex().IsMatch(hostname));
            }
        });

        app.UseHttpLogging();
        app.UseRouting();
        app.UseHttpMetrics();
        app.UseAuthorization();
    }
    

    [GeneratedRegex(@"^(https:\/\/(gestdown\.info|addictedproxy\.pages\.dev|dev\.addictedproxy\.pages\.dev)|.*\.gestdown\.info)$")]
    private static partial Regex CorsOriginRegex();
}