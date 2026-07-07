using BasicJira.Application.Common.Interfaces;
using BasicJira.Application.Projects.Commands.CreateProject;
using BasicJira.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); // api endpointlerini barındıran controller sınıflarını "ProjectsController" sisteme dahil edilir.

builder.Services.AddEndpointsApiExplorer(); 
                                            // swaggerin arka planda api rotalarını bulup swagger dokümantasyonunu
                                            // oluşturabilmesi için gerekli olan servisi sisteme dahil eder.
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(                                                   // EF Core'un SQL Server ile çalışabilmesi için gerekli olan servisi sisteme dahil eder.
        builder.Configuration.GetConnectionString("DefaultConnection"));    // appsettings.json dosyasında tanımlı olan "DefaultConnection" isimli connection stringi alır ve EF Core'a iletir.
});     

builder.Services.AddScoped<IAppDbContext>(provider =>           // application katmanının SQL yapısını direkt bilmemesi için IAppDbContext interface'i ile AppDbContext sınıfını sisteme dahil eder.    
    provider.GetRequiredService<AppDbContext>());


builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateProjectCommand).Assembly);
});

builder.Services.AddOpenApi();

var app = builder.Build();  // middleware aşaması başlar

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();       //swagger üzerinden execute edildiğinde arka planda oluşan HTTP URL isteğinin, gidipi doğru controllerı uygun metodu (HttpPost) bulmasını saglar

app.Run();


