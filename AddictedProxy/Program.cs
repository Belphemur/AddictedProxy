#region

using System.Reflection;
using AddictedProxy.Caching.Bootstrap;
using AddictedProxy.Controllers.Bootstrap;
using AddictedProxy.Culture.Bootstrap;
using AddictedProxy.Database.Bootstrap;
using AddictedProxy.Database.Context;
using AddictedProxy.Image.Bootstrap;
using AddictedProxy.OneTimeMigration.Bootstrap;
using AddictedProxy.Services.Job.Exception;
using AddictedProxy.Stats.Popularity.Bootstrap;
using AddictedProxy.Storage.Caching.Bootstrap;
using AddictedProxy.Storage.Store.Compression.Bootstrap;
using AddictedProxy.Upstream.Boostrap;
using Compressor.Bootstrap;
using Hangfire.Storage;
using InversionOfControl.Service.Bootstrap;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Sentry;
using Performance.Bootstrap;
using Performance.Model;
using Prometheus;
using TvMovieDatabaseClient.Bootstrap;

#endregion

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen(options =>
       {
           // using System.Reflection;
           var executingAssembly = Assembly.GetExecutingAssembly();
           var xmlFilename = $"{executingAssembly.GetName().Name}.xml";
           options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
           if (builder.Environment.IsProduction())
           {
               options.AddServer(new OpenApiServer
               {
                   Url = "https://api.gestdown.info",
                   Description = "Production"
               });
           }

           options.SwaggerDoc("v1", new OpenApiInfo
           {
               Title = "Gestdown: Addicted Proxy",
               Description = "Provide a full api to search and download subtitles from Addic7ed website.",
               Version = executingAssembly.GetName().Version!.ToString(3)
           });
       })
       .AddEndpointsApiExplorer();

builder.Configuration.AddEnvironmentVariables("A7D_");

//Add our own bootstrapping
var currentAssemblies = new[]
{
    typeof(BootstrapController).Assembly,
    typeof(BootstrapDatabase).Assembly,
    typeof(BootstrapCompressor).Assembly,
    typeof(BootstrapAddictedServices).Assembly,
    typeof(BootstrapPerformanceOpenTelemetry).Assembly,
    typeof(BootstrapStatsPopularityShow).Assembly,
    typeof(BootstrapTMDB).Assembly,
    typeof(BootstrapRedisCaching).Assembly,
    typeof(BootstrapCulture).Assembly,
    typeof(BootstrapStorageCaching).Assembly,
    typeof(BootstrapMigration).Assembly,
    typeof(BootstrapImage).Assembly,
    typeof(BootstrapStoreCompression).Assembly
};

builder.Services
       .AddBootstrap(builder.Configuration, currentAssemblies);

builder.Host.UseSystemd();

Metrics.SuppressDefaultMetrics();

var perf = builder.Configuration.GetSection("Performance").Get<PerformanceConfig>()!;
if (perf.Type == PerformanceConfig.BackendType.Sentry)
{
    builder.WebHost.UseSentry(sentryBuilder =>
    {
        sentryBuilder.Dsn = perf.Endpoint;
        sentryBuilder.TracesSampleRate = perf.SampleRate;
        sentryBuilder.Release = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "1.0.0";
        sentryBuilder.Environment = builder.Environment.EnvironmentName;
        sentryBuilder.Debug = builder.Environment.IsDevelopment();
        sentryBuilder.AddExceptionFilterForType<OperationCanceledException>();
        sentryBuilder.AddExceptionFilterForType<TaskCanceledException>();
        sentryBuilder.AddExceptionFilterForType<RetryJobException>();
        sentryBuilder.AddExceptionFilterForType<DistributedLockTimeoutException>();
    });
}

var app = builder.Build();

var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All,
    ForwardedForHeaderName = app.Configuration.GetSection("Http:RealIpHeader").Get<string>()!,
};
forwardedHeadersOptions.KnownProxies.Clear();
forwardedHeadersOptions.KnownNetworks.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

app.UseBootstrap(currentAssemblies);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage()
       .UseSwaggerUI(options => options.RoutePrefix = "api");
} 

app.UseSwagger(options => options.RouteTemplate = "api/{documentName}/swagger.{json|yaml}");

{
    await using var serviceScope = app.Services.CreateAsyncScope();
    await using var dbContext = serviceScope.ServiceProvider.GetRequiredService<EntityContext>();

    await dbContext.Database.MigrateAsync();
}

app.Services.GetRequiredService<ILogger<Program>>().LogInformation("Application version: {version}", Assembly.GetExecutingAssembly().GetName().Version!.ToString(3));


app.Run();