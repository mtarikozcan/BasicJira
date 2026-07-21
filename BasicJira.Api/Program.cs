using BasicJira.Api.Configuration;
using BasicJira.Api.Middleware;
using BasicJira.Application;
using BasicJira.Infrastructure;
using Microsoft.OpenApi;
using Serilog;
using Elastic.Serilog.Sinks;
using Elastic.Ingest.Elasticsearch.DataStreams;

var builder = WebApplication.CreateBuilder(args);

await builder.Configuration.AddVaultSecretsAsync();

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()

        .WriteTo.Console()

        .WriteTo.Elasticsearch(
            new[] { new Uri("http://localhost:9200") },
            options =>
            {
                options.DataStream =
                    new DataStreamName("logs", "basicjira", "api");
            });
});



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT token giriniz."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] =
                new List<string>()
        });
});

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();     // 

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();