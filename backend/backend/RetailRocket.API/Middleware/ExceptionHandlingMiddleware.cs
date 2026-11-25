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
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogWarning("Multiple error/warnings in queue.");
        }
        catch (Exception ex)
        {
            var (status, message) = Map(ex);
            
            if ((int)status >= 500)
                _logger.LogError(ex, "Unhandled error on {Path}", context.Request.Path);
            else
                _logger.LogWarning("{Type} -> {Status}: {Message}", ex.GetType().Name, (int)status, message);
            
            if (context.Response.HasStarted) return;

            context.Response.StatusCode = (int)status;
            await context.Response.WriteAsJsonAsync(new { statusCode = (int)status, message });
        }
    }

    private static (HttpStatusCode Status, string Message) Map(Exception ex) => ex switch
    {
        
        ValidationException => (HttpStatusCode.BadRequest, ex.Message),
        AuthenticationException => (HttpStatusCode.Unauthorized, ex.Message),
        AuthorizationException => (HttpStatusCode.Forbidden, ex.Message),
        BannedUserException => (HttpStatusCode.Forbidden, ex.Message),
        NotFoundException => (HttpStatusCode.NotFound, ex.Message),
        EntityConflictException => (HttpStatusCode.Conflict, ex.Message),
        EmailDeliveryException => (HttpStatusCode.ServiceUnavailable,
            "Mail service temporarily unavailable. Please try again later."),
        TimeoutException => (HttpStatusCode.ServiceUnavailable,
            "Service temporarily unavailable. Please try again later."),
        _ => (HttpStatusCode.InternalServerError, "An unexpected error has occurred.")
    };
}