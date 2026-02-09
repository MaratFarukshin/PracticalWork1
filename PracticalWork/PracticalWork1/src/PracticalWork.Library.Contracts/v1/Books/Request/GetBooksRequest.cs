using PracticalWork.Library.Contracts.v1.Enums;

namespace PracticalWork.Library.Contracts.v1.Books.Request;

/// <summary>
/// Запрос на получение списка книг
/// </summary>
/// <param name="Status">Фильтр по статусу (опционально)</param>
/// <param name="Category">Фильтр по категории (опционально)</param>
/// <param name="Author">Фильтр по автору (опционально)</param>
/// <param name="PageNumber">Номер страницы (начиная с 1)</param>
/// <param name="PageSize">Размер страницы</param>
public sealed record GetBooksRequest(
    BookStatus? Status = null,
    BookCategory? Category = null,
    string Author = null,
    int PageNumber = 1,
    int PageSize = 10
);

