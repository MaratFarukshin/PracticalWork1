using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Application.Queries.Library;

/// <summary>
/// Запрос получения списка книг библиотеки
/// </summary>
public sealed record GetLibraryBooksQuery(
    BookCategory? Category = null,
    string Author = null,
    int PageNumber = 1,
    int PageSize = 10
) : IQuery<PagedResult<BookWithBorrowInfo>>;

