using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Application.Commands.Books;

/// <summary>
/// Команда обновления книги
/// </summary>
public sealed record UpdateBookCommand(Guid Id, Book Book) : ICommand;

