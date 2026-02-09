namespace PracticalWork.Library.Contracts.v1.Library.Response;

/// <summary>
/// Ответ со списком книг библиотеки
/// </summary>
/// <param name="Books">Список книг</param>
/// <param name="TotalCount">Общее количество книг</param>
/// <param name="PageNumber">Номер текущей страницы</param>
/// <param name="PageSize">Размер страницы</param>
/// <param name="TotalPages">Общее количество страниц</param>
public sealed record GetLibraryBooksResponse(
    IReadOnlyList<LibraryBookListItemResponse> Books,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);

/// <summary>
/// Элемент списка книг библиотеки с информацией о выдаче
/// </summary>
/// <param name="Id">Идентификатор книги</param>
/// <param name="Title">Название книги</param>
/// <param name="Category">Категория книги</param>
/// <param name="Authors">Авторы</param>
/// <param name="Status">Статус</param>
/// <param name="IsArchived">В архиве</param>
/// <param name="ReaderId">Идентификатор читателя, которому выдана книга (null, если не выдана)</param>
/// <param name="BorrowDate">Дата выдачи книги (null, если не выдана)</param>
/// <param name="DueDate">Срок возврата книги (null, если не выдана)</param>
public sealed record LibraryBookListItemResponse(
    Guid Id,
    string Title,
    PracticalWork.Library.Contracts.v1.Enums.BookCategory Category,
    IReadOnlyList<string> Authors,
    PracticalWork.Library.Contracts.v1.Enums.BookStatus Status,
    bool IsArchived,
    Guid? ReaderId,
    DateOnly? BorrowDate,
    DateOnly? DueDate
);

