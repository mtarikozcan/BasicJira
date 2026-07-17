using System;
using System.Collections.Generic;
using System.Text;
using BasicJira.Application.Common.Interfaces;
using BasicJira.Infrastructure.Persistence;
using BasicJira.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BasicJira.Infrastructure.Repositories;
using BasicJira.Infrastructure.Email;

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

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IUnitOfWork>(provider =>
            provider.GetRequiredService<AppDbContext>());

        services.Configure<EmailSettings>(
        configuration.GetSection(EmailSettings.SectionName));

        services.AddTransient<IEmailService, EmailService>();

        return services;
    }
}
