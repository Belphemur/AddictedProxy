using AddictedProxy;
using AddictedProxy.Database;
using AddictedProxy.Database.Context;
using AddictedProxy.Services.Saver;
using Job.Scheduler.Scheduler;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSystemd();

new Startup().ConfigureServices(builder.Services);

var app = builder.Build();
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