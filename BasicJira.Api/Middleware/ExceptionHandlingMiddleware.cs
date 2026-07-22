using BasicJira.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BasicJira.Api.Middleware;

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

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var (statusCode, title, detail, logLevel) = exception switch
        {
            ValidationException =>
            (
                StatusCodes.Status400BadRequest,
                "Validation Failed",
                "One or more validation errors occurred.",
                LogLevel.Warning
            ),

            ForbiddenException forbiddenException =>
            (
                StatusCodes.Status403Forbidden,
                "Forbidden",
                forbiddenException.Message,
                LogLevel.Warning
            ),

            NotFoundException notFoundException =>
            (
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                notFoundException.Message,
                LogLevel.Warning
            ),

            ConflictException conflictException =>
            (
                StatusCodes.Status409Conflict,
                "Conflict",
                conflictException.Message,
                LogLevel.Warning
            ),

            _ =>
            (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred.",
                LogLevel.Error
            )
        };

        _logger.Log(
            logLevel,
            exception,
            "Exception occurred while processing {Method} {Path}. TraceId: {TraceId}",
            context.Request.Method,
            context.Request.Path,
            context.TraceIdentifier);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        if (exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(error => error.ErrorMessage)
                        .Distinct()
                        .ToArray());

            var validationProblemDetails =
                new HttpValidationProblemDetails(errors)
                {
                    Status = statusCode,
                    Title = title,
                    Detail = detail,
                    Instance = context.Request.Path
                };

            validationProblemDetails.Extensions["traceId"] =
                context.TraceIdentifier;

            await context.Response.WriteAsJsonAsync(
                validationProblemDetails);

            return;
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        problemDetails.Extensions["traceId"] =
            context.TraceIdentifier;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}