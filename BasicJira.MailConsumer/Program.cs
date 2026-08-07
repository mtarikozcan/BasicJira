using BasicJira.MailConsumer.Interfaces;
using BasicJira.MailConsumer.Persistence;
using BasicJira.MailConsumer.Services;
using BasicJira.MailConsumer.Settings;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Retry;
using System.Net.Sockets;

var builder = Host.CreateApplicationBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MailConsumerDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddResiliencePipeline(
    "email-retry",
    pipelineBuilder =>
    {
        pipelineBuilder.AddRetry(new RetryStrategyOptions
        {
            MaxRetryAttempts = 2,

            Delay = TimeSpan.FromSeconds(1),

            BackoffType = DelayBackoffType.Exponential,

            UseJitter = false,
                
            ShouldHandle = new PredicateBuilder()
                .Handle<IOException>()
                .Handle<SocketException>()
                .Handle<TimeoutException>()
                .Handle<SmtpProtocolException>(),

            OnRetry = arguments =>
            {
                var exception = arguments.Outcome.Exception;

                Console.WriteLine(
                    "E-posta gönderimi başarısız. " +
                    "Retry: {0}/2, Bekleme: {1} saniye, Hata: {2}",
                    arguments.AttemptNumber + 1,
                    arguments.RetryDelay.TotalSeconds,
                    exception?.GetType().Name);

                return ValueTask.CompletedTask;
            }
        });
    });

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


builder.Services.AddTransient<IEmailService, EmailService>();   //

builder.Services.AddHostedService<RabbitMqMailConsumerService>();

var host = builder.Build();

await host.RunAsync();