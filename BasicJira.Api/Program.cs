using BasicJira.Api.Configuration;
using BasicJira.Api.Middleware;
using BasicJira.Application;
using BasicJira.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

await builder.Configuration.AddVaultSecretsAsync();     // infra structure servisleri register edilmeden önce çalıştırılıyor -> email service ve 
                                                        // dbcontext config okuma yapmadan önce vaulttan secretların yüklenmiş olması gerekir.

// Controllers
builder.Services.AddControllers();

// OpenAPI / Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Clean Architecture registrations
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();