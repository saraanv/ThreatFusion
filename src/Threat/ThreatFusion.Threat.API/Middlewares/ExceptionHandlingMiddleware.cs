using System.Net;
using System.Text.Json;

namespace ThreatFusion.Threat.API.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException exception)
        {
            _logger.LogWarning(
                exception,
                "Unauthorized request.");

            await WriteErrorResponseAsync(
                context,
                HttpStatusCode.Unauthorized,
                "Unauthorized",
                exception.Message);
        }
        catch (KeyNotFoundException exception)
        {
            _logger.LogWarning(
                exception,
                "Resource was not found.");

            await WriteErrorResponseAsync(
                context,
                HttpStatusCode.NotFound,
                "NotFound",
                exception.Message);
        }
        catch (ArgumentException exception)
        {
            _logger.LogWarning(
                exception,
                "Invalid request.");

            await WriteErrorResponseAsync(
                context,
                HttpStatusCode.BadRequest,
                "BadRequest",
                exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled server exception.");

            await WriteErrorResponseAsync(
                context,
                HttpStatusCode.InternalServerError,
                "InternalServerError",
                "An unexpected server error occurred.");
        }
    }

    private static async Task WriteErrorResponseAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string errorCode,
        string message)
    {
        context.Response.StatusCode =
            (int)statusCode;

        context.Response.ContentType =
            "application/json";

        var response =
            new
            {
                ErrorCode = errorCode,
                Message = message,
                TraceId =
                    context.TraceIdentifier
            };

        var json =
            JsonSerializer.Serialize(
                response);

        await context.Response.WriteAsync(
            json);
    }
}