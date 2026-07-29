using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using WeatherService.Api.Middleware;
using Xunit;

namespace WeatherService.Api.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_NoException_CallsNextAndLeavesResponseUntouched()
    {
        var context = new DefaultHttpContext();
        var nextCalled = false;
        var middleware = new ExceptionHandlingMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        }, NullLogger<ExceptionHandlingMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        Assert.True(nextCalled);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_DownstreamThrows_ReturnsGenericErrorWithoutStackTrace()
    {
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };
        var middleware = new ExceptionHandlingMiddleware(_ =>
            throw new InvalidOperationException("boom with sensitive stack details"),
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        using var json = JsonDocument.Parse(body);
        Assert.Equal("An unexpected error occurred.", json.RootElement.GetProperty("message").GetString());
        Assert.DoesNotContain("boom", body);
    }
}
