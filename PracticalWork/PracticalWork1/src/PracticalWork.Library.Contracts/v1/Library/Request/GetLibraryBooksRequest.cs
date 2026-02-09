using PracticalWork.Library.Contracts.v1.Enums;

namespace PracticalWork.Library.Contracts.v1.Library.Request;

/// <summary>
/// Запрос на получение списка книг библиотеки
/// </summary>
/// <param name="Category">Фильтр по категории (опционально)</param>
/// <param name="Author">Фильтр по автору (опционально)</param>
/// <param name="PageNumber">Номер страницы (начиная с 1)</param>
/// <param name="PageSize">Размер страницы</param>
public sealed record GetLibraryBooksRequest(
    BookCategory? Category = null,
    string Author = null,
    int PageNumber = 1,
    int PageSize = 10
);

