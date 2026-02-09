using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Application.Queries.Books;

/// <summary>
/// Запрос получения деталей книги
/// </summary>
public sealed record GetBookDetailsQuery(Guid Id) : IQuery<Book>;

