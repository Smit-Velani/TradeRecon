using System.Net;
using System.Text.Json;

namespace TradeRecon.Api.Middleware;

public class ExceptionHandlingMiddleware
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

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var correlationId = context.TraceIdentifier;

            _logger.LogError(
                ex,
                "Unhandled exception. CorrelationId: {CorrelationId}",
                correlationId);

            context.Response.StatusCode =
                (int)HttpStatusCode.InternalServerError;

            context.Response.ContentType = "application/json";

            var response = new
            {
                StatusCode = 500,
                Message = "An unexpected error occurred.",
                CorrelationId = correlationId
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}
