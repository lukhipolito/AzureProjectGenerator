using AzureProjectTemplate.API.Settings;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzureProjectTemplate.API.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly ApplicationInsightsSettings _applicationInsights;

        public ErrorHandlerMiddleware(IOptions<ApplicationInsightsSettings> options)
        {
            _applicationInsights = options.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            var telemetry = new TelemetryClient(new TelemetryConfiguration(_applicationInsights.InstrumentationKey));

            telemetry.Context.Operation.Id = Guid.NewGuid().ToString();

            var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;

            if (ex == null)
            {
                return;
            }

            telemetry.TrackException(ex);

            var problemDetails = new ProblemDetails
            {
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Instance = context.Request.Path.Value,
                Detail = ex.InnerException is null ?
                    $"{ex.Message}" :
                    $"{ex.Message} | {ex.InnerException}"
            };

            context.Response.StatusCode = problemDetails.Status.Value;
            context.Response.ContentType = "application/problem+json";

            using var writer = new Utf8JsonWriter(context.Response.Body);
            JsonSerializer.Serialize(writer, problemDetails);
            await writer.FlushAsync();
        }
    }
}
