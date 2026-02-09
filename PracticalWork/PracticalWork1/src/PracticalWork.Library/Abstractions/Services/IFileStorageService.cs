namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Сервис для работы с файловым хранилищем
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Загрузить файл в хранилище
    /// </summary>
    /// <param name="bucketName">Имя bucket</param>
    /// <param name="objectName">Имя объекта (путь к файлу)</param>
    /// <param name="fileStream">Поток файла</param>
    /// <param name="contentType">MIME-тип файла</param>
    /// <returns>Путь к загруженному файлу</returns>
    Task<string> UploadFileAsync(string bucketName, string objectName, Stream fileStream, string contentType);

    /// <summary>
    /// Удалить файл из хранилища
    /// </summary>
    /// <param name="bucketName">Имя bucket</param>
    /// <param name="objectName">Имя объекта (путь к файлу)</param>
    Task DeleteFileAsync(string bucketName, string objectName);
    
    /// <summary>
    /// Получить URL для доступа к файлу
    /// </summary>
    /// <param name="filePath">Путь к файлу (возвращается из UploadFileAsync)</param>
    /// <returns>URL для доступа к файлу</returns>
    string GetFileUrl(string filePath);
}

