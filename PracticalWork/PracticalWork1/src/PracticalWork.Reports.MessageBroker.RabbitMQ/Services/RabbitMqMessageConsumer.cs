using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PracticalWork.Library.Events;
using PracticalWork.Reports.Abstractions.Services;
using PracticalWork.Reports.Abstractions.Storage;
using PracticalWork.Reports.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PracticalWork.Reports.MessageBroker.RabbitMQ.Services;

/// <summary>
/// RabbitMQ consumer для подписки на события библиотеки
/// </summary>
public sealed class RabbitMqMessageConsumer : BackgroundService, IMessageConsumer
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMqMessageConsumer> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private const string ExchangeName = "library_events";
    private const string QueueName = "reports_activity_logs";

    public RabbitMqMessageConsumer(
        string connectionString,
        ILogger<RabbitMqMessageConsumer> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;

        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Объявляем exchange и queue
        _channel.ExchangeDeclare(
            exchange: ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        _channel.QueueDeclare(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        // Подписываемся на все события библиотеки
        _channel.QueueBind(
            queue: QueueName,
            exchange: ExchangeName,
            routingKey: "book.*");

        _channel.QueueBind(
            queue: QueueName,
            exchange: ExchangeName,
            routingKey: "reader.*");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                _logger.LogInformation("Получено сообщение: {RoutingKey}", routingKey);

                // Десериализация базового события
                var baseEvent = JsonSerializer.Deserialize<BaseLibraryEvent>(message);
                if (baseEvent == null)
                {
                    _logger.LogWarning("Не удалось десериализовать событие");
                    _channel.BasicAck(ea.DeliveryTag, false);
                    return;
                }

                // Создание scope для работы с репозиторием (scoped-сервис)
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var activityLogRepository = scope.ServiceProvider.GetRequiredService<IActivityLogRepository>();

                    // Создание лога активности
                    var activityLog = CreateActivityLogFromEvent(baseEvent, routingKey, message);
                    await activityLogRepository.CreateAsync(activityLog, stoppingToken);

                    _logger.LogInformation(
                        "Лог активности создан: {EventType}, {EventId}",
                        baseEvent.EventType,
                        baseEvent.EventId);
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке сообщения");
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(
            queue: QueueName,
            autoAck: false,
            consumer: consumer);

        return Task.CompletedTask;
    }

    private static ActivityLog CreateActivityLogFromEvent(BaseLibraryEvent @event, string routingKey, string metadata)
    {
        var eventType = MapEventTypeToInt(@event.EventType);
        var (bookId, readerId) = ExtractIdsFromEvent(@event, routingKey);

        return new ActivityLog
        {
            Id = Guid.NewGuid(),
            ExternalBookId = bookId,
            ExternalReaderId = readerId,
            EventType = eventType,
            EventDate = @event.OccurredOn,
            Metadata = metadata,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static int MapEventTypeToInt(string eventType)
    {
        return eventType switch
        {
            "BookCreated" => (int)EventType.BookCreated,
            "BookArchived" => (int)EventType.BookArchived,
            "ReaderCreated" => (int)EventType.ReaderCreated,
            "ReaderClosed" => (int)EventType.ReaderClosed,
            "BookBorrowed" => (int)EventType.BookBorrowed,
            "BookReturned" => (int)EventType.BookReturned,
            _ => 0
        };
    }

    private static (Guid? bookId, Guid? readerId) ExtractIdsFromEvent(BaseLibraryEvent @event, string routingKey)
    {
        Guid? bookId = null;
        Guid? readerId = null;

        // Парсим JSON для извлечения ID
        try
        {
            var jsonDoc = JsonDocument.Parse(JsonSerializer.Serialize(@event));
            
            if (jsonDoc.RootElement.TryGetProperty("BookId", out var bookIdElement))
            {
                bookId = bookIdElement.GetGuid();
            }

            if (jsonDoc.RootElement.TryGetProperty("ReaderId", out var readerIdElement))
            {
                readerId = readerIdElement.GetGuid();
            }
        }
        catch
        {
            // Игнорируем ошибки парсинга
        }

        return (bookId, readerId);
    }

    public Task StartConsumingAsync(CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(cancellationToken);
    }

    public Task StopConsumingAsync(CancellationToken cancellationToken = default)
    {
        _channel?.Close();
        _connection?.Close();
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
        base.Dispose();
    }
}

