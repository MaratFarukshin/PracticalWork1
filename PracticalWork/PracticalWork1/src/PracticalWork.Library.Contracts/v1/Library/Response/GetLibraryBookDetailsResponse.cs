namespace PracticalWork.Library.Contracts.v1.Library.Response;

/// <summary>
/// Ответ с детальной информацией по книге библиотеки
/// </summary>
/// <param name="Id">Идентификатор книги</param>
/// <param name="Title">Название книги</param>
/// <param name="Category">Категория книги</param>
/// <param name="Authors">Авторы</param>
/// <param name="Description">Краткое описание книги</param>
/// <param name="Year">Год издания</param>
/// <param name="CoverImageUrl">URL для доступа к обложке</param>
/// <param name="Status">Статус</param>
/// <param name="IsArchived">В архиве</param>
public sealed record GetLibraryBookDetailsResponse(
    Guid Id,
    string Title,
    PracticalWork.Library.Contracts.v1.Enums.BookCategory Category,
    IReadOnlyList<string> Authors,
    string Description,
    int Year,
    string CoverImageUrl,
    PracticalWork.Library.Contracts.v1.Enums.BookStatus Status,
    bool IsArchived
);

