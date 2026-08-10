#region

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using AddictedProxy.Controllers.Config;
using AddictedProxy.Controllers.Rest.Serializer;
using AddictedProxy.Services.Middleware;
using InversionOfControl.Model;
using Microsoft.Extensions.Options;
using Prometheus;

#endregion

namespace AddictedProxy.Controllers.Bootstrap;

public class BootstrapController : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
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
        services.Configure<CorsConfig>(configuration.GetSection(CorsConfig.SectionName));
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        if (app is IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllers();
        }

        var corsConfig = app.ApplicationServices.GetRequiredService<IOptions<CorsConfig>>().Value;

        app.UseCors(policyBuilder =>
        {
            policyBuilder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("Content-Disposition")
                .WithExposedHeaders("sentry-trace")
                .WithExposedHeaders("baggage");
            if (app.ApplicationServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                policyBuilder.SetIsOriginAllowed(_ => true);
            }
            else
            {
                policyBuilder
                    .WithOrigins(corsConfig.AllowedOrigins)
                    .SetIsOriginAllowedToAllowWildcardSubdomains();
            }
        });

        app.UseHttpLogging();
        app.UseRouting();
        app.UseHttpMetrics();
        app.UseAuthorization();
    }
}
