using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PracticalWork.Library.Abstractions.Services;
using RabbitMQ.Client;

namespace PracticalWork.Library.MessageBroker.RabbitMQ.Services;

/// <summary>
/// Реализация публикатора сообщений через RabbitMQ
/// </summary>
public sealed class RabbitMqMessagePublisher : IMessagePublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMqMessagePublisher> _logger;
    private const string ExchangeName = "library_events";

    public RabbitMqMessagePublisher(string connectionString, ILogger<RabbitMqMessagePublisher> logger)
    {
        _logger = logger;
        
        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        // Объявляем exchange для событий библиотеки
        _channel.ExchangeDeclare(
            exchange: ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);
    }

    public Task PublishAsync<T>(T message, string? routingKey = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var key = routingKey ?? typeof(T).Name.ToLowerInvariant();

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            _channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: key,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Сообщение опубликовано: {EventType}, RoutingKey: {RoutingKey}", typeof(T).Name, key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при публикации сообщения {EventType}", typeof(T).Name);
            throw;
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}

