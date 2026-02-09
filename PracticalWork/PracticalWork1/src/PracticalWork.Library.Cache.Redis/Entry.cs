using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Cache.Redis.Services;

namespace PracticalWork.Library.Cache.Redis;

public static class Entry
{
    /// <summary>
    /// Регистрация зависимостей для распределенного Cache
    /// </summary>
    public static IServiceCollection AddCache(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = configuration["App:Redis:RedisCacheConnection"];
        var prefix = configuration["App:Redis:RedisCachePrefix"];

        // Регистрация сервиса кэша
        serviceCollection.AddSingleton<ICacheService, CacheService>();

        // TODO: Реализовать подключение к Redis и настройку сервисов

        return serviceCollection;
    }
}

