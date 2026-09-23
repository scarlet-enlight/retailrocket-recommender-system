using System.Net;
using System.Text.Json;

namespace RetailRocket.API.Middleware;

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
            _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error has occured.");
            await HandleExceptionAsync(context, ex);
        }
    }

    public async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            KeyNotFoundException => (
                StatusCode: HttpStatusCode.NotFound,
                Message: exception.Message
                ),
            UnauthorizedAccessException => (
                StatusCode: HttpStatusCode.Unauthorized,
                Message: "Unauthorized."
                ),
            ArgumentException => (
                StatusCode: HttpStatusCode.BadRequest,
                Message: exception.Message
                ),
            _ => (
                StatusCode: HttpStatusCode.BadRequest,
                Message: "An unexcepted error has occured."
                ),
        };
        
        context.Response.StatusCode = (int)response.StatusCode;

        var json = JsonSerializer.Serialize(new
        {
            statusCode = context.Response.StatusCode,
            message = response.Message
        });
        
        await context.Response.WriteAsync(json);
    }
}