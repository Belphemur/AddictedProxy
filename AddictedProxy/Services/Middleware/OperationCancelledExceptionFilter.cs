using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace AddictedProxy.Services.Middleware
{
    public class OperationCancelledExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;

        public OperationCancelledExceptionFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<OperationCancelledExceptionFilter>();
        }

        public override void OnException(ExceptionContext context)
        {
            if (!(context.Exception is OperationCanceledException))
                return;
            _logger.LogInformation("Request was cancelled");
            context.ExceptionHandled = true;
            context.Result           = new StatusCodeResult(400);
        }
    }
}