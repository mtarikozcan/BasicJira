using BasicJira.Contracts.Messages;
using BasicJira.MailConsumer.Interfaces;
using BasicJira.MailConsumer.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace BasicJira.MailConsumer.Services;

public sealed class RabbitMqMailConsumerService : BackgroundService
{
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<RabbitMqMailConsumerService> _logger;
    private readonly IEmailService _emailService;

    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqMailConsumerService(
    IOptions<RabbitMqSettings> options,
    ILogger<RabbitMqMailConsumerService> logger,
    IEmailService emailService)
    {
        _settings = options.Value;
        _logger = logger;
        _emailService = emailService;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
            ClientProvidedName = "BasicJira.MailConsumer",
            AutomaticRecoveryEnabled = true
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(
            cancellationToken: stoppingToken);

        await ConfigureTopologyAsync(stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            await HandleMessageAsync(eventArgs, stoppingToken);
        };

        await _channel.BasicConsumeAsync(
            queue: _settings.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation(
            "RabbitMQ consumer çalışıyor. Exchange: {Exchange}, Queue: {Queue}, RoutingKey: {RoutingKey}",
            _settings.ExchangeName,
            _settings.QueueName,
            _settings.RoutingKey);

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }

    private async Task ConfigureTopologyAsync(
        CancellationToken cancellationToken)
    {
        if (_channel is null)
        {
            throw new InvalidOperationException(
                "RabbitMQ channel oluşturulmadı.");
        }

        await _channel.ExchangeDeclareAsync(
            exchange: _settings.ExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: _settings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await _channel.QueueBindAsync(
            queue: _settings.QueueName,
            exchange: _settings.ExchangeName,
            routingKey: _settings.RoutingKey,
            arguments: null,
            cancellationToken: cancellationToken);

        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: cancellationToken);
    }

    private async Task HandleMessageAsync(
        BasicDeliverEventArgs eventArgs,
        CancellationToken cancellationToken)
    {
        if (_channel is null)
        {
            throw new InvalidOperationException(
                "RabbitMQ channel oluşturulmadı.");
        }

        try
        {
            var body = eventArgs.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            var message = JsonSerializer.Deserialize<SendEmailMessage>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (message is null)
            {
                throw new JsonException(
                    "RabbitMQ mesajı deserialize edilemedi.");
            }

            PrintMessage(message);

            await _emailService.SendAsync(
                message,
                cancellationToken);
            await _channel.BasicAckAsync(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "RabbitMQ mesajı işlendi ve ACK gönderildi. MessageId: {MessageId}",
                message.MessageId);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            // Uygulama kontrollü şekilde kapanıyor.
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "RabbitMQ mesajı işlenirken hata oluştu. DeliveryTag: {DeliveryTag}",
                eventArgs.DeliveryTag);

            await _channel.BasicNackAsync(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                requeue: false,
                cancellationToken: CancellationToken.None);
        }
    }

    private static void PrintMessage(
        SendEmailMessage message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine();
        Console.WriteLine(
            "══════════════════════════════════════════════════════════════");
        Console.WriteLine(
            "                    YENİ RABBITMQ MESAJI");
        Console.WriteLine(
            "══════════════════════════════════════════════════════════════");
        Console.ResetColor();

        WriteField(
            "Alınma",
            DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));

        WriteField(
            "Message Id",
            message.MessageId.ToString());

        WriteField(
            "Alıcı",
            message.Recipient);

        WriteField(
            "Konu",
            message.Subject);

        WriteField(
            "İçerik",
            message.Body);

        WriteField(
            "Oluşturulma",
            message.CreatedAtUtc
                .ToLocalTime()
                .ToString("dd.MM.yyyy HH:mm:ss"));
    }

    private static void WriteField(
        string label,
        string? value)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"  {label,-12}: ");

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(value ?? "-");

        Console.ResetColor();
    }

    public override async Task StopAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "RabbitMQ consumer kapatılıyor.");

        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();

        base.Dispose();
    }
}