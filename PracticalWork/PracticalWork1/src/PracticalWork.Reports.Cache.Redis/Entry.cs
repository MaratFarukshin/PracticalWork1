using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Reports.Abstractions.Services;
using PracticalWork.Reports.Cache.Redis.Services;

namespace PracticalWork.Reports.Cache.Redis;

public static class Entry
{
    public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICacheService, CacheService>();
        return services;
    }
}

