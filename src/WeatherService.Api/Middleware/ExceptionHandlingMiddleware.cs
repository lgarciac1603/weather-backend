using System.Net;
using System.Text.Json;

namespace WeatherService.Api.Middleware;

/// <summary>
/// Catches unhandled exceptions from the request pipeline, logs the full error, and returns
/// a generic JSON error response so callers never see a raw stack trace.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception processing {Path}", context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var error = new { statusCode = context.Response.StatusCode, message = "An unexpected error occurred." };
            await context.Response.WriteAsync(JsonSerializer.Serialize(error));
        }
    }
}