namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Сервис для валидации файлов
/// </summary>
public interface IFileValidationService
{
    /// <summary>
    /// Валидировать файл обложки
    /// </summary>
    /// <param name="fileStream">Поток файла</param>
    /// <param name="fileName">Имя файла</param>
    /// <param name="contentType">MIME-тип файла</param>
    /// <param name="maxSizeInBytes">Максимальный размер файла в байтах</param>
    /// <returns>Результат валидации</returns>
    FileValidationResult ValidateCoverImage(Stream fileStream, string fileName, string contentType, long maxSizeInBytes);
}

/// <summary>
/// Результат валидации файла
/// </summary>
public sealed class FileValidationResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; }

    public static FileValidationResult Success() => new() { IsValid = true };
    public static FileValidationResult Failure(string errorMessage) => new() { IsValid = false, ErrorMessage = errorMessage };
}

