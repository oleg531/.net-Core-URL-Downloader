namespace Downloader.Core.Exceptions
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Http;

    public static class LoggingExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggingExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoggingExceptionHandlerMiddleware>();
        }
    }

    public class LoggingExceptionHandlerMiddleware
    {
        public LoggingExceptionHandlerMiddleware(RequestDelegate next)
        {

        }

        public async Task Invoke(HttpContext context, IServiceProvider services)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync("Internal server error");

            var exception = context.Features.Get<IExceptionHandlerFeature>();
            if (exception?.Error != null)
            {
                //TODO log exception here
            }
        }
    }
}