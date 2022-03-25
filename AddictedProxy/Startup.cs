using System.Net;
using AddictedProxy.Database;
using AddictedProxy.Database.Context;
using AddictedProxy.Database.Repositories;
using AddictedProxy.Model.Config;
using AddictedProxy.Services.Addic7ed;
using AddictedProxy.Services.Job;
using AddictedProxy.Services.Middleware;
using AddictedProxy.Services.Saver;
using AngleSharp.Html.Parser;
using Job.Scheduler.Builder;
using Job.Scheduler.Scheduler;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using Polly.RateLimit;
using Polly.Timeout;
using Z.EntityFramework.Extensions;

namespace AddictedProxy
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHtmlParser, HtmlParser>();
            services.AddSingleton<Parser>();

            services.AddHttpClient<IAddic7edClient, Addic7edClient>()
                .SetHandlerLifetime(TimeSpan.FromHours(1))
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(Policy.RateLimitAsync<HttpResponseMessage>(
                    3, TimeSpan.FromSeconds(1), 10
                ));

            services.AddHttpClient<IAddic7edDownloader, Addic7edDownloader>()
                .ConfigurePrimaryHttpMessageHandler(_ =>
                {
                    var proxyUrl = Environment.GetEnvironmentVariable("PROXY_URL") ?? throw new InvalidOperationException("No proxy set");
                    var proxyUri = new Uri(proxyUrl.Trim('"'));
                    var userSplit = proxyUri.UserInfo.Split(":");
                    return new HttpClientHandler
                    {
                        Proxy = new WebProxy
                        {
                            Address =  new Uri(proxyUri.Scheme + "://" + proxyUri.Authority),
                            Credentials = new NetworkCredential(userSplit[0], userSplit[1])
                        }
                    };
                })
                .SetHandlerLifetime(TimeSpan.FromHours(1))
                .AddPolicyHandler(GetRetryPolicy());

            services.AddControllers()
                .AddMvcOptions(options => options.Filters.Add<OperationCancelledExceptionFilter>());
            services.AddLogging(opt => { opt.AddConsole(c => { c.TimestampFormat = "[HH:mm:ss] "; }); });

            services.AddSingleton<IJobRunnerBuilder, JobRunnerBuilder>();
            services.AddSingleton<IJobScheduler, JobScheduler>();
            services.AddSingleton<MainCreds>();
            services.AddDbContext<EntityContext>();
            EntityFrameworkManager.ContextFactory = context => new EntityContext(new DbContextOptions<EntityContext>());
            services.AddScoped<ITvShowRepository, TvShowRepository>();
            services.AddScoped<IAddictedSaver, AddictedSaver>();
            services.AddScoped<ISeasonRepository, SeasonRepository>();
            services.AddScoped<IEpisodeRepository, EpisodeRepository>();
            services.AddScoped<ISubtitleRepository, SubtitleRepository>();
            services.AddHostedService<JobSchedulerHostedService>();
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            var jitterer = new Random();
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .Or<RateLimitRejectedException>()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound || msg.StatusCode == HttpStatusCode.Forbidden)
                .WaitAndRetryAsync(8, // exponential back-off plus some jitter
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                    + TimeSpan.FromMilliseconds(jitterer.Next(0, 300))
                ).WrapAsync(Policy.TimeoutAsync(10));
        }
    }
}