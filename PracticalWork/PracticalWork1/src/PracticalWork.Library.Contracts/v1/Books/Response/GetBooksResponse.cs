namespace PracticalWork.Library.Contracts.v1.Books.Response;

/// <summary>
/// Ответ со списком книг
/// </summary>
/// <param name="Books">Список книг</param>
/// <param name="TotalCount">Общее количество книг</param>
/// <param name="PageNumber">Номер текущей страницы</param>
/// <param name="PageSize">Размер страницы</param>
/// <param name="TotalPages">Общее количество страниц</param>
public sealed record GetBooksResponse(
    IReadOnlyList<BookListItemResponse> Books,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);

/// <summary>
/// Элемент списка книг
/// </summary>
/// <param name="Id">Идентификатор книги</param>
/// <param name="Title">Название книги</param>
/// <param name="Category">Категория книги</param>
/// <param name="Authors">Авторы</param>
/// <param name="Status">Статус</param>
/// <param name="IsArchived">В архиве</param>
public sealed record BookListItemResponse(
    Guid Id,
    string Title,
    PracticalWork.Library.Contracts.v1.Enums.BookCategory Category,
    IReadOnlyList<string> Authors,
    PracticalWork.Library.Contracts.v1.Enums.BookStatus Status,
    bool IsArchived
);

