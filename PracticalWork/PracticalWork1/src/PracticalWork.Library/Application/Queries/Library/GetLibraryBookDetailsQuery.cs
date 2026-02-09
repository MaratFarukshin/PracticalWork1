using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Application.Queries.Library;

/// <summary>
/// Запрос получения деталей книги библиотеки
/// </summary>
public sealed record GetLibraryBookDetailsQuery(string IdOrTitle) : IQuery<BookDetails>;

