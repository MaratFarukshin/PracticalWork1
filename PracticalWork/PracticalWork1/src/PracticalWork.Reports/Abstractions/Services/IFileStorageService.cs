namespace PracticalWork.Reports.Abstractions.Services;

/// <summary>
/// Интерфейс для работы с хранилищем файлов
/// </summary>
public interface IFileStorageService
{
    Task<string> UploadFileAsync(string bucketName, string objectName, Stream fileStream, string contentType);
    Task<bool> FileExistsAsync(string bucketName, string objectName);
    string GetFileUrl(string filePath);
    Task<string> GenerateSignedUrlAsync(string bucketName, string objectName, TimeSpan expiration);
    Task<IReadOnlyList<string>> ListFilesAsync(string bucketName, string prefix = null!);
}

