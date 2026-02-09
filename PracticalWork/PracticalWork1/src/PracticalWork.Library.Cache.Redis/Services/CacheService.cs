using System.Text.Json;
using PracticalWork.Library.Abstractions.Services;

namespace PracticalWork.Library.Cache.Redis.Services;

/// <summary>
/// Реализация сервиса кэша на основе Redis
/// </summary>
public sealed class CacheService : ICacheService
{
    // TODO: Реализовать подключение к Redis и методы работы с кэшем
    // Для полноценной реализации потребуется StackExchange.Redis
    // Временная реализация использует in-memory словарь для демонстрации логики

    private readonly Dictionary<string, (string value, DateTime expiration)> _cache = new();

    public Task<T> GetAsync<T>(string key) where T : class
    {
        // TODO: Реализовать получение из Redis
        // var value = await _redisDatabase.StringGetAsync(key);
        // if (!value.HasValue) return null;
        // return JsonSerializer.Deserialize<T>(value);

        if (!_cache.TryGetValue(key, out var cached) || cached.expiration < DateTime.UtcNow)
        {
            _cache.Remove(key);
            return Task.FromResult<T>(null);
        }

        try
        {
            var result = JsonSerializer.Deserialize<T>(cached.value);
            return Task.FromResult(result);
        }
        catch
        {
            return Task.FromResult<T>(null);
        }
    }

    public Task SetAsync<T>(string key, T value, TimeSpan expiration) where T : class
    {
        // TODO: Реализовать сохранение в Redis
        // var json = JsonSerializer.Serialize(value);
        // await _redisDatabase.StringSetAsync(key, json, expiration);

        var json = JsonSerializer.Serialize(value);
        var expirationTime = DateTime.UtcNow.Add(expiration);
        _cache[key] = (json, expirationTime);
        return Task.CompletedTask;
    }

    public Task InvalidateAsync(string key)
    {
        // TODO: Реализовать удаление ключа из Redis
        // await _redisDatabase.KeyDeleteAsync(key);
        _cache.Remove(key);
        return Task.CompletedTask;
    }

    public Task InvalidateByPatternAsync(string pattern)
    {
        // TODO: Реализовать удаление ключей по паттерну из Redis
        // var keys = _server.Keys(pattern: pattern);
        // foreach (var key in keys)
        //     await _redisDatabase.KeyDeleteAsync(key);
        
        var keysToRemove = _cache.Keys.Where(k => k.Contains(pattern.Replace("*", ""))).ToList();
        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);
        }
        return Task.CompletedTask;
    }
}

