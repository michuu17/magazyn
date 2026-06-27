using System.Text.Json;
using WarehouseApi.Exceptions;

namespace WarehouseApi.Middleware;

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
        catch (NotFoundException ex)
        {
            await WriteResponseAsync(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (DomainException ex)
        {
            await WriteResponseAsync(context, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Nieobsluzony wyjatek podczas przetwarzania zadania.");
            await WriteResponseAsync(context, StatusCodes.Status500InternalServerError, "Wystapil nieoczekiwany blad serwera.");
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var payload = new
        {
            status = statusCode,
            message
        };

        var json = JsonSerializer.Serialize(payload);
        await context.Response.WriteAsync(json);
    }
}
