using System;
using System.Collections.Generic;
using System.Text;
using BasicJira.Application.Common.Interfaces;
using BasicJira.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BasicJira.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IAppDbContext>(provider =>
            provider.GetRequiredService<AppDbContext>());

        return services;
    }
}