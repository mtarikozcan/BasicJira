using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

const string exchangeName = "basicjira.mail.exchange";      // message topology de kullanılcak isimler tanımlandı. 
const string queueName = "basicjira.mail.queue";
const string routingKey = "mail.send";

var factory = new ConnectionFactory     // rabbit mq bağlantısı için factory oluşturuyoruz. bu da bi secret, ilerde vaulta taşınabilir.
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
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);    

    Console.WriteLine();
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Yeni mesaj alındı:");
    Console.WriteLine(message);

    await channel.BasicAckAsync(
        deliveryTag: eventArgs.DeliveryTag,
        multiple: false);

    Console.WriteLine("Mesaj ACK ile onaylandı.");
};

await channel.BasicConsumeAsync(
    queue: queueName,
    autoAck: false,
    consumer: consumer);

Console.WriteLine("BasicJira.MailConsumer çalışıyor.");
Console.WriteLine($"Exchange   : {exchangeName}");
Console.WriteLine($"Queue      : {queueName}");
Console.WriteLine($"Routing key: {routingKey}");
Console.WriteLine("Mesaj bekleniyor...");
Console.WriteLine("Kapatmak için Ctrl+C.");

var shutdown = new TaskCompletionSource(
    TaskCreationOptions.RunContinuationsAsynchronously);

Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    shutdown.TrySetResult();
};

await shutdown.Task;