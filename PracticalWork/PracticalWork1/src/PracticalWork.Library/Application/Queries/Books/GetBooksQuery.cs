using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Application.Queries.Books;

/// <summary>
/// Запрос получения списка книг
/// </summary>
public sealed record GetBooksQuery(
    BookStatus? Status = null,
    BookCategory? Category = null,
    string Author = null,
    int PageNumber = 1,
    int PageSize = 10
) : IQuery<PagedResult<Book>>;

