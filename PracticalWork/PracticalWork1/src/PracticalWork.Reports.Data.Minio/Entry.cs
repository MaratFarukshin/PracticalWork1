using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Reports.Abstractions.Services;
using PracticalWork.Reports.Data.Minio.Services;

namespace PracticalWork.Reports.Data.Minio;

public static class Entry
{
    public static IServiceCollection AddMinioFileStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var baseUrl = configuration["App:Minio:BaseUrl"] ?? "http://localhost:9000";
        services.AddSingleton<IFileStorageService>(sp => new FileStorageService(baseUrl));
        return services;
    }
}

