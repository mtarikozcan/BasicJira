using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using BasicJira.Application.Common.Behaviors;
using FluentValidation;
using MediatR;

namespace BasicJira.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                Assembly.GetExecutingAssembly());
        });

        // registers all validators inside application assembly
        services.AddValidatorsFromAssembly(
            Assembly.GetExecutingAssembly()
            );

        // runs validation before the request reaches the handler
        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>)
            );

        return services;
    }
}