using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Application.Commands.Books;

/// <summary>
/// Команда архивирования книги
/// </summary>
public sealed record ArchiveBookCommand(Guid Id) : ICommand<Book>;

