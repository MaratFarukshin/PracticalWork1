using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Data.Minio.Services;

namespace PracticalWork.Library.Data.Minio;

public static class Entry
{
    /// <summary>
    /// Регистрация зависимостей для хранилища документов
    /// </summary>
    public static IServiceCollection AddMinioFileStorage(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = configuration["App:Minio:MinioFileStorageConnection"];

        // Регистрация сервиса файлового хранилища
        serviceCollection.AddSingleton<IFileStorageService, FileStorageService>();

        // TODO: Реализовать подключение к MinIO и настройку клиента

        return serviceCollection;
    }
}
