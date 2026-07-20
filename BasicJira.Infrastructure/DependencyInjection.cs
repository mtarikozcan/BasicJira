using BasicJira.Application.Common.Interfaces;
using BasicJira.Application.Common.Settings;
using BasicJira.Infrastructure.Email;
using BasicJira.Infrastructure.Persistence;
using BasicJira.Infrastructure.Repositories;
using BasicJira.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;



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

        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        var jwtSettings = configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>()
            ?? throw new InvalidOperationException(
                "JWT ayarları bulunamadı.");

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    jwtSettings.SecretKey)),

                        RoleClaimType = ClaimTypes.Role
                    };
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
