namespace PracticalWork.Reports.Abstractions.Services;

/// <summary>
/// Интерфейс для подписки на сообщения из брокера
/// </summary>
public interface IMessageConsumer
{
    /// <summary>
    /// Начать прослушивание сообщений
    /// </summary>
    Task StartConsumingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Остановить прослушивание сообщений
    /// </summary>
    Task StopConsumingAsync(CancellationToken cancellationToken = default);
}

