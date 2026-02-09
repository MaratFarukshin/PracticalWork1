namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Интерфейс для публикации сообщений в брокер сообщений
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Публикует сообщение в брокер
    /// </summary>
    /// <typeparam name="T">Тип сообщения</typeparam>
    /// <param name="message">Сообщение для публикации</param>
    /// <param name="routingKey">Ключ маршрутизации (опционально)</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task PublishAsync<T>(T message, string routingKey = null, CancellationToken cancellationToken = default) where T : class;
}

