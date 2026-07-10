using FluentValidation;
using System.Net;
using System.Text.Json;

namespace BasicJira.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)  // bu metod requestin tüm yolculuğunu try-catch bloğuna alır
    {
        try
        {
            await _next(context);
        }

        catch (ValidationException ex)     // fluentvalidationdan fırlattığımız validationexception catch 
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var response = new
            {
                title = "Validation Failed",
                status = 400,
                errors = ex.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage)
                    )
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }

        catch (Exception)       //system error 
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                title = "Internal Server Error",
                status = 500
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}