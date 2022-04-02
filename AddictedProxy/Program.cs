using System.ComponentModel;
using AddictedProxy;
using AddictedProxy.Controllers.Bootstrap;
using AddictedProxy.Database;
using AddictedProxy.Database.Context;
using AddictedProxy.Services.Saver;
using InversionOfControl.Service;
using InversionOfControl.Service.Bootstrap;
using Job.Scheduler.Scheduler;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen().AddEndpointsApiExplorer();

//Add our own bootstrapping
var currentAssembly = typeof(BootstrapController).Assembly;

builder.Services
       .AddBootstrapEnvironmentVar(currentAssembly)
       .AddBootstrap(currentAssembly);
builder.Host.UseSystemd();

var app = builder.Build();
app.UseHttpLogging();
{
    await using var serviceScope = app.Services.CreateAsyncScope();
    await using var dbContext = serviceScope.ServiceProvider.GetRequiredService<EntityContext>();

// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    await dbContext.Database.MigrateAsync();
}


app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();