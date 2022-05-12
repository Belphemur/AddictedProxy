#region

using System.Reflection;
using AddictedProxy.Controllers.Rest.Bootstrap;
using AddictedProxy.Database.Bootstrap;
using AddictedProxy.Database.Context;
using AddictedProxy.Storage.Bootstrap;
using AddictedProxy.Upstream.Boostrap;
using InversionOfControl.Service.Bootstrap;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

#endregion

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen(options =>
       {
           // using System.Reflection;
           var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
           options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
           options.SwaggerDoc("v1", new OpenApiInfo
           {
               Title = "Addicted Proxy",
               Description = "Provide a full system to search and download subtitles from Addi7ed website.",
               Version = Assembly.GetExecutingAssembly().GetName().Version!.ToString(3)
           });
       })
       .AddEndpointsApiExplorer();

//Add our own bootstrapping
var currentAssemblies = new[]
{
    typeof(BootstrapController).Assembly,
    typeof(BootstrapDatabase).Assembly,
    typeof(BootstrapCompressor).Assembly,
    typeof(BootstrapAddictedServices).Assembly
};

builder.Services
       .AddBootstrap(builder.Configuration, currentAssemblies);

builder.Host.UseSystemd();
builder.WebHost.UseSentry(sentryBuilder =>
{
    sentryBuilder.Dsn = Environment.GetEnvironmentVariable("SENTRY_DSN");
#if DEBUG
    sentryBuilder.Debug = true;
#endif
    sentryBuilder.TracesSampleRate = 1.0;
});

var app = builder.Build();

app.UseBootstrap(currentAssemblies);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger(options => options.RouteTemplate = "api/{documentName}/swagger.{json|yaml}")
   .UseSwaggerUI(options => options.RoutePrefix = "api");


app.UseSentryTracing();

app.UseCors(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("Content-Disposition"));

{
    await using var serviceScope = app.Services.CreateAsyncScope();
    await using var dbContext = serviceScope.ServiceProvider.GetRequiredService<EntityContext>();

    await dbContext.Database.MigrateAsync();
}

app.Run();