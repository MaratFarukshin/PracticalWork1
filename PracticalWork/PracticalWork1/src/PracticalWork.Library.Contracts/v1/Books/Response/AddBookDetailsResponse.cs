namespace PracticalWork.Library.Contracts.v1.Books.Response;

/// <summary>
/// Ответ на добавление деталей книги
/// </summary>
/// <param name="Id">Идентификатор книги</param>
/// <param name="CoverImagePath">Путь к изображению обложки</param>
public sealed record AddBookDetailsResponse(
    Guid Id,
    string CoverImagePath
);

