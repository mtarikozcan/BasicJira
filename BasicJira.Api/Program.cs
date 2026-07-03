using BasicJira.Application.Common.Interfaces;
using BasicJira.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// registers AppDbContext in the Dependency Injection container.
// whenever IAppDbContext is requested, AppDbContext will be provided.

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")
        );
    // UseSqlServer methodu, Entity Framework Core'un SQL Server veritabanı sağlayıcısını kullanmasını sağlar.
});


// maps the abstraciton (IAppDbContext) to its concrete implementation

builder.Services.AddScoped<IAppDbContext>(provider =>
    provider.GetRequiredService<AppDbContext>());



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
