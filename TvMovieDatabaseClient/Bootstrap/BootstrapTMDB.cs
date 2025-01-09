﻿using System;
using System.Net;
using System.Net.Http;
using InversionOfControl.Model;
using InversionOfControl.Service.EnvironmentVariable.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TvMovieDatabaseClient.Bootstrap.EnvVar;
using TvMovieDatabaseClient.Service;

namespace TvMovieDatabaseClient.Bootstrap;

public class BootstrapTMDB : IBootstrap, IBootstrapEnvironmentVariable<TmdbConfig, TmdbConfigParser>
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<ITMDBClient, TMDBClient>(client => client.BaseAddress = new Uri("https://api.themoviedb.org/3/"))
            .SetHandlerLifetime(TimeSpan.FromMinutes(10))
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.All
            })
            .AddStandardResilienceHandler();
    }

    public EnvVarRegistration<TmdbConfig, TmdbConfigParser> EnvVarRegistration { get; } = new("TMDB_APIKEY");
}