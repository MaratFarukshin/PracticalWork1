namespace PracticalWork.Reports.Abstractions.Services;

/// <summary>
/// Интерфейс для работы с кэшем
/// </summary>
public interface ICacheService
{
    Task<T> GetAsync<T>(string key) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan expiration) where T : class;
    Task InvalidateAsync(string key);
    Task InvalidateByPatternAsync(string pattern);
}

