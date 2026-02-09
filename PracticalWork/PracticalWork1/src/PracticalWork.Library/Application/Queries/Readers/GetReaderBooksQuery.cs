using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Application.Queries.Readers;

/// <summary>
/// Запрос получения списка книг читателя
/// </summary>
public sealed record GetReaderBooksQuery(Guid ReaderId) : IQuery<IReadOnlyList<BorrowedBook>>;

