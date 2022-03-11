using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AddictedProxy.Model.Config;
using AddictedProxy.Services.Addic7ed;
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
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHtmlParser, HtmlParser>();
            services.AddSingleton<Parser>();

            services.AddHttpClient<IAddic7edClient, Addic7edClient>()
                    .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
                    {
                        Proxy = new WebProxy
                        {
                            Address = new Uri("***REMOVED***"),
                            Credentials = new NetworkCredential("***REMOVED***", "***REMOVED***")
                        }
                    })
                    .SetHandlerLifetime(TimeSpan.FromHours(1))
                    .AddPolicyHandler(GetRetryPolicy());

            services.AddControllers()
                    .AddMvcOptions(options => options.Filters.Add<OperationCancelledExceptionFilter>());

            services.AddMemoryCache();

            services.AddLogging(opt => { opt.AddConsole(c => { c.TimestampFormat = "[HH:mm:ss] "; }); });
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