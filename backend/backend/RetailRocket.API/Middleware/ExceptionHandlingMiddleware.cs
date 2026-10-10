using System.Net;
using System.Text.Json;
using RetailRocket.Domain.Exceptions;

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
            await _next(context);
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
            ValidationException => ( // 400 for no/bad data
                StatusCode: HttpStatusCode.BadRequest,
                Message: exception.Message
            ),
            AuthenticationException => ( // 401 for non-logged users
                StatusCode: HttpStatusCode.Unauthorized,
                Message: exception.Message
            ),
            NotFoundException => ( // 404 for entities
                StatusCode: HttpStatusCode.NotFound,
                Message: exception.Message
                ),
            ArgumentException => ( // default 400
                    StatusCode: HttpStatusCode.BadRequest,
                    Message: exception.Message
                ),
            UnauthorizedAccessException => ( // default 401
                    StatusCode: HttpStatusCode.Unauthorized,
                    Message: exception.Message
                ),
            KeyNotFoundException => ( // default 404
                StatusCode: HttpStatusCode.NotFound,
                Message: exception.Message
                ),
            _ => (
                StatusCode: HttpStatusCode.InternalServerError, // default 500
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