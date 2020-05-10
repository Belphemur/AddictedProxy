using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using AddictedProxy.Model.Config;
using AddictedProxy.Services.Addic7ed;
using AddictedProxy.Services.Addic7ed.Exception;
using AddictedProxy.Services.Caching;
using AddictedProxy.Services.Middleware;
using AddictedProxy.Services.Proxy;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace AddictedProxy
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider => MemoryCache.Default);
            services.AddSingleton<ICachingService, CachingService>();
            services.AddTransient(provider =>
            {
                var proxies = provider.GetService<IProxyGetter>().GetWebProxiesAsync(CancellationToken.None).Result;
                return new MutliWebProxy(proxies);
            });
            services.AddSingleton<IHtmlParser, HtmlParser>();
            services.AddSingleton<Parser>();

            services.AddHttpClient<IProxyGetter, ProxyGetter>()
                    .SetHandlerLifetime(TimeSpan.FromDays(1))
                    .AddPolicyHandler(GetRetryPolicy());

            services.AddHttpClient<IAddic7edClient, Addic7edClient>()
                    .ConfigurePrimaryHttpMessageHandler(provider => new HttpClientHandler
                    {
                        Proxy = provider.GetService<MutliWebProxy>()
                    })
                    .SetHandlerLifetime(TimeSpan.FromHours(1))
                    .AddPolicyHandler(GetRetryPolicy());

            services.AddControllers()
                    .AddMvcOptions(options => options.Filters.Add<OperationCancelledExceptionFilter>());

            services.AddLogging(opt =>
            {
                opt.AddConsole(c =>
                {
                    c.TimestampFormat = "[HH:mm:ss] ";
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            var jitterer = new Random();
            return HttpPolicyExtensions
                   .HandleTransientHttpError()
                   .Or<TimeoutRejectedException>()
                   .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound || msg.StatusCode == HttpStatusCode.Forbidden)
                   .WaitAndRetryAsync(8, // exponential back-off plus some jitter
                       retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                       + TimeSpan.FromMilliseconds(jitterer.Next(0, 300))
                   ).WrapAsync(Policy.TimeoutAsync(10));
        }
    }
}