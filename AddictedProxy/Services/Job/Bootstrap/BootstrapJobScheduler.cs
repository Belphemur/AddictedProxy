﻿#region

using AddictedProxy.Caching.Bootstrap.EnvVar;
using AddictedProxy.Services.Job.Service;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using InversionOfControl.Model;
using StackExchange.Redis;

#endregion

namespace AddictedProxy.Services.Job.Bootstrap;

public class BootstrapJobScheduler : IBootstrap, IBootstrapApp
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection("Redis").Get<RedisConfig>()!;

        services.AddHangfire(conf => conf
                                     .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                                     .UseSimpleAssemblyNameTypeSerializer()
                                     .UseRecommendedSerializerSettings()
                                     .UseRedisStorage(ConnectionMultiplexer.Connect(config.Connection)));

        services.AddHangfireServer(options => options.Queues = new[] { "refresh-one-show", "fetch-subtitles", "store-subtitle" });
        services.AddHostedService<SchedulerHostedService>();
    }

    public void ConfigureApp(IApplicationBuilder app)
    {
        app.UseHangfireDashboard(options: new DashboardOptions
        {
            Authorization = new[]
            {
                new BasicAuthAuthorizationFilter(
                    new BasicAuthAuthorizationFilterOptions
                    {
                        // Require secure connection for dashboard
                        RequireSsl = false,
                        SslRedirect = false,
                        // Case sensitive login checking
                        LoginCaseSensitive = true,
                        // Users
                        Users = new[]
                        {
                            new BasicAuthAuthorizationUser
                            {
                                Login = "admin",
                                // Password as plain text, SHA1 will be used
                                Password = new byte[] { 0x96, 0x25, 0x68, 0x2a, 0x0a, 0xe0, 0x32, 0x52, 0x0a, 0x76, 0x0e, 0x64, 0xe5, 0x6a, 0x5d, 0xac, 0xba, 0xe7, 0x4b, 0xaa }
                            }
                        }
                    })
            }
        });
    }
}