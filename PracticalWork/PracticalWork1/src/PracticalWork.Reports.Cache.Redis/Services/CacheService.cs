using System.Collections.Concurrent;
using System.Text.Json;
using PracticalWork.Reports.Abstractions.Services;

namespace PracticalWork.Reports.Cache.Redis.Services;

/// <summary>
/// Реализация кэша через Redis (временная реализация через in-memory для демонстрации)
/// </summary>
public sealed class CacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, (string value, DateTime expiration)> _cache = new();

    public Task<T> GetAsync<T>(string key) where T : class
    {
        if (_cache.TryGetValue(key, out var entry) && entry.expiration > DateTime.UtcNow)
        {
            return Task.FromResult(JsonSerializer.Deserialize<T>(entry.value))!;
        }

        _cache.TryRemove(key, out _);
        return Task.FromResult<T>(null!);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan expiration) where T : class
    {
        var serialized = JsonSerializer.Serialize(value);
        _cache[key] = (serialized, DateTime.UtcNow.Add(expiration));
        return Task.CompletedTask;
    }

    public Task InvalidateAsync(string key)
    {
        _cache.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public Task InvalidateByPatternAsync(string pattern)
    {
        var keysToRemove = _cache.Keys.Where(k => k.Contains(pattern.Replace("*", ""))).ToList();
        foreach (var key in keysToRemove)
        {
            _cache.TryRemove(key, out _);
        }
        return Task.CompletedTask;
    }
}

