using System.Net;
using MendSync.Application.DTOs.Common;

namespace MendSync.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Mend API returned 401 Unauthorized");
            await WriteErrorResponse(context, 401, "Unauthorized. Token may have expired. Please login again.");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
        {
            _logger.LogWarning("Mend API returned 403 Forbidden");
            await WriteErrorResponse(context, 403, "Forbidden. You do not have permission to perform this action.");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Mend API returned 404 Not Found: {Message}", ex.Message);
            await WriteErrorResponse(context, 404, "The requested resource was not found.");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
        {
            _logger.LogWarning("Mend API returned 429 Too Many Requests");
            context.Response.Headers.Append("Retry-After", "60");
            await WriteErrorResponse(context, 429, "Too many requests. Please retry after 60 seconds.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Invalid operation: {Message}", ex.Message);
            await WriteErrorResponse(context, 400, ex.Message);
        }
        catch (TimeoutException ex)
        {
            _logger.LogWarning("Operation timed out: {Message}", ex.Message);
            await WriteErrorResponse(context, 408, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception processing request {Method} {Path}",
                context.Request.Method, context.Request.Path);
            await WriteErrorResponse(context, 500, "An internal server error occurred.");
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        var response = ApiResponse<object>.Fail(message);
        await context.Response.WriteAsJsonAsync(response);
    }
}
