using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Stride.Middleware
{
    public class EnvironmentIndicatorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _environment;

        public EnvironmentIndicatorMiddleware(RequestDelegate next, IWebHostEnvironment environment)
        {
            _next = next;
            _environment = environment.EnvironmentName;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Add a response header indicating the environment
            context.Response.Headers.Add("X-Environment", _environment);
            
            // Continue with the pipeline
            await _next(context);
        }
    }

    // Extension method for middleware registration
    public static class EnvironmentIndicatorMiddlewareExtensions
    {
        public static IApplicationBuilder UseEnvironmentIndicator(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EnvironmentIndicatorMiddleware>();
        }
    }
}