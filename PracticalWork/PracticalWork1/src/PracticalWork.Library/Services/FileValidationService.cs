using PracticalWork.Library.Abstractions.Services;

namespace PracticalWork.Library.Services;

/// <summary>
/// Сервис для валидации файлов
/// </summary>
public sealed class FileValidationService : IFileValidationService
{
    private static readonly string[] AllowedImageFormats = { "image/jpeg", "image/jpg", "image/png", "image/webp" };
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB

    public FileValidationResult ValidateCoverImage(Stream fileStream, string fileName, string contentType, long maxSizeInBytes)
    {
        // Проверка размера файла
        if (fileStream.Length > maxSizeInBytes)
        {
            var maxSizeInMB = maxSizeInBytes / (1024.0 * 1024.0);
            return FileValidationResult.Failure($"Размер файла превышает максимально допустимый размер {maxSizeInMB:F2} МБ.");
        }

        if (fileStream.Length == 0)
        {
            return FileValidationResult.Failure("Файл не может быть пустым.");
        }

        // Проверка формата по расширению
        var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
        {
            return FileValidationResult.Failure($"Недопустимый формат файла. Разрешенные форматы: {string.Join(", ", AllowedExtensions)}");
        }

        // Проверка MIME-типа
        if (string.IsNullOrEmpty(contentType) || !AllowedImageFormats.Contains(contentType.ToLowerInvariant()))
        {
            return FileValidationResult.Failure($"Недопустимый MIME-тип файла. Разрешенные типы: {string.Join(", ", AllowedImageFormats)}");
        }

        // Проверка магических байтов (сигнатура файла)
        if (!ValidateFileSignature(fileStream, extension))
        {
            return FileValidationResult.Failure("Содержимое файла не соответствует заявленному формату.");
        }

        return FileValidationResult.Success();
    }

    private static bool ValidateFileSignature(Stream fileStream, string extension)
    {
        var position = fileStream.Position;
        fileStream.Position = 0;

        var buffer = new byte[8];
        var bytesRead = fileStream.Read(buffer, 0, buffer.Length);
        fileStream.Position = position;

        if (bytesRead < 2)
            return false;

        return extension switch
        {
            ".jpg" or ".jpeg" => buffer[0] == 0xFF && buffer[1] == 0xD8,
            ".png" => buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47,
            ".webp" => buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46,
            _ => false
        };
    }
}

