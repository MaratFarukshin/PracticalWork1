using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Application.Commands.Books;

/// <summary>
/// Команда создания книги
/// </summary>
public sealed record CreateBookCommand(Book Book) : ICommand<Guid>;

