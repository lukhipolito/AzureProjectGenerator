using Microsoft.AspNetCore.Builder;
using AzureProjectTemplate.API.Middlewares;

namespace AzureProjectTemplate.API.Extensions
{
    public static class LogExtensions
    {
        public static IApplicationBuilder UseLogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogMiddleware>();
        }
    }
}
