namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Сервис для работы с кэшем
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Получить значение из кэша
    /// </summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="key">Ключ кэша</param>
    /// <returns>Значение из кэша или null, если не найдено</returns>
    Task<T> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// Сохранить значение в кэш
    /// </summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="key">Ключ кэша</param>
    /// <param name="value">Значение для сохранения</param>
    /// <param name="expiration">Время жизни кэша</param>
    Task SetAsync<T>(string key, T value, TimeSpan expiration) where T : class;

    /// <summary>
    /// Инвалидировать кэш по ключу
    /// </summary>
    /// <param name="key">Ключ кэша</param>
    Task InvalidateAsync(string key);

    /// <summary>
    /// Инвалидировать кэш по паттерну ключа
    /// </summary>
    /// <param name="pattern">Паттерн ключа кэша</param>
    Task InvalidateByPatternAsync(string pattern);
}

