using PracticalWork.Reports.Abstractions.Services;

namespace PracticalWork.Reports.Data.Minio.Services;

/// <summary>
/// Реализация сервиса для работы с MinIO
/// </summary>
public sealed class FileStorageService : IFileStorageService
{
    private readonly string _baseUrl;

    public FileStorageService(string baseUrl = "http://localhost:9000")
    {
        _baseUrl = baseUrl;
    }

    public Task<string> UploadFileAsync(string bucketName, string objectName, Stream fileStream, string contentType)
    {
        // Временная реализация - в реальности здесь будет загрузка в MinIO
        var path = $"{bucketName}/{objectName}";
        return Task.FromResult(path);
    }

    public Task<bool> FileExistsAsync(string bucketName, string objectName)
    {
        // Временная реализация - в реальности здесь будет проверка существования файла в MinIO
        return Task.FromResult(true);
    }

    public string GetFileUrl(string filePath)
    {
        return $"{_baseUrl}/{filePath}";
    }

    public Task<string> GenerateSignedUrlAsync(string bucketName, string objectName, TimeSpan expiration)
    {
        // Временная реализация - в реальности здесь будет генерация signed URL
        var url = $"{_baseUrl}/{bucketName}/{objectName}?expires={DateTime.UtcNow.Add(expiration)}";
        return Task.FromResult(url);
    }

    public Task<IReadOnlyList<string>> ListFilesAsync(string bucketName, string prefix = null!)
    {
        // Временная реализация - в реальности здесь будет запрос списка файлов из MinIO
        var files = new List<string>();
        if (prefix != null)
        {
            files.Add($"{prefix}/report_20240101_20240131_abc123.csv");
        }
        else
        {
            files.Add("reports/2024/01/report_20240101_20240131_abc123.csv");
            files.Add("reports/2024/02/report_20240201_20240228_def456.csv");
        }
        return Task.FromResult<IReadOnlyList<string>>(files);
    }
}

