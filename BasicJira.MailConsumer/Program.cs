using BasicJira.MailConsumer.Services;
using BasicJira.MailConsumer.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddOptions<RabbitMqSettings>()
    .Bind(builder.Configuration.GetSection("RabbitMq"))
    .Validate(
        settings => !string.IsNullOrWhiteSpace(settings.HostName),
        "RabbitMQ HostName boş olamaz.")
    .Validate(
        settings => !string.IsNullOrWhiteSpace(settings.UserName),
        "RabbitMQ UserName boş olamaz.")
    .Validate(
        settings => !string.IsNullOrWhiteSpace(settings.Password),
        "RabbitMQ Password boş olamaz.")
    .ValidateOnStart();

builder.Services
    .AddOptions<EmailSettings>()
    .Bind(builder.Configuration.GetSection("EmailSettings"))
    .Validate(
        settings => !string.IsNullOrWhiteSpace(settings.Host),
        "SMTP Host boş olamaz.")
    .Validate(
        settings => !string.IsNullOrWhiteSpace(settings.SenderEmail),
        "Gönderici e-posta adresi boş olamaz.")
    .Validate(
        settings => !string.IsNullOrWhiteSpace(settings.Password),
        "SMTP şifresi boş olamaz.")
    .ValidateOnStart();

builder.Services.AddHostedService<RabbitMqMailConsumerService>();

var host = builder.Build();

await host.RunAsync();