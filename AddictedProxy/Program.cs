#region

using System.Reflection;
using AddictedProxy.Controllers.Bootstrap;
using AddictedProxy.Database.Bootstrap;
using AddictedProxy.Database.Context;
using InversionOfControl.Service.Bootstrap;
using Microsoft.EntityFrameworkCore;

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
}).AddEndpointsApiExplorer();

//Add our own bootstrapping
var currentAssemblies = new []
{
    typeof(BootstrapController).Assembly,
    typeof(BootstrapDatabase).Assembly
};

builder.Services
       .AddBootstrapEnvironmentVar(currentAssemblies)
       .AddBootstrap(currentAssemblies);
builder.Host.UseSystemd();

var app = builder.Build();
app.UseHttpLogging();
{
    await using var serviceScope = app.Services.CreateAsyncScope();
    await using var dbContext = serviceScope.ServiceProvider.GetRequiredService<EntityContext>();


    await dbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger().UseSwaggerUI();


app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();