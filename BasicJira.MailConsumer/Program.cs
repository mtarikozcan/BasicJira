using System.Text;
using System.Text.Json;
using BasicJira.Contracts.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

const string exchangeName = "basicjira.mail.exchange";
const string queueName = "basicjira.mail.queue";
const string routingKey = "mail.send";

var factory = new ConnectionFactory
{
    HostName = "localhost",
    Port = 5672,
    UserName = "basicjira",
    Password = "BasicJira123!",
    ClientProvidedName = "BasicJira.MailConsumer"
};

await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(
    exchange: exchangeName,
    type: ExchangeType.Direct,
    durable: true,
    autoDelete: false);

await channel.QueueDeclareAsync(
    queue: queueName,
    durable: true,
    exclusive: false,
    autoDelete: false);

await channel.QueueBindAsync(
    queue: queueName,
    exchange: exchangeName,
    routingKey: routingKey);

await channel.BasicQosAsync(
    prefetchSize: 0,
    prefetchCount: 1,
    global: false);

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (_, eventArgs) =>
{
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
            throw new Exception("Mesaj deserialize edilemedi.");

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine();
        Console.WriteLine("══════════════════════════════════════════════════════════════");
        Console.WriteLine("                    YENİ RABBITMQ MESAJI");
        Console.WriteLine("══════════════════════════════════════════════════════════════");
        Console.ResetColor();

        WriteField("Alınma", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
        WriteField("Message Id", message.MessageId.ToString());
        WriteField("Alıcı", message.Recipient);
        WriteField("Konu", message.Subject);
        WriteField("İçerik", message.Body);
        WriteField(
            "Oluşturulma",
            message.CreatedAtUtc.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss"));

        await channel.BasicAckAsync(
            deliveryTag: eventArgs.DeliveryTag,
            multiple: false);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine();
        Console.WriteLine("✓ Mesaj başarıyla işlendi.");
        Console.WriteLine("✓ RabbitMQ ACK gönderildi.");
        Console.ResetColor();

        Console.WriteLine("──────────────────────────────────────────────────────────────");
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine();
        Console.WriteLine("✗ Mesaj işlenirken hata oluştu.");
        Console.WriteLine($"Hata: {ex.Message}");
        Console.ResetColor();

        await channel.BasicNackAsync(
            deliveryTag: eventArgs.DeliveryTag,
            multiple: false,
            requeue: false);
    }
};

await channel.BasicConsumeAsync(
    queue: queueName,
    autoAck: false,
    consumer: consumer);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("BasicJira.MailConsumer çalışıyor.");
Console.ResetColor();

Console.WriteLine($"Exchange    : {exchangeName}");
Console.WriteLine($"Queue       : {queueName}");
Console.WriteLine($"Routing Key : {routingKey}");
Console.WriteLine();
Console.WriteLine("Mesaj bekleniyor...");
Console.WriteLine("Çıkmak için Ctrl + C");

var shutdown = new TaskCompletionSource(
    TaskCreationOptions.RunContinuationsAsynchronously);

Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    shutdown.TrySetResult();
};

await shutdown.Task;

static void WriteField(string label, string? value)
{
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write($"  {label,-12}: ");

    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(value ?? "-");

    Console.ResetColor();
}