using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Application.Commands.Library;

/// <summary>
/// Команда выдачи книги
/// </summary>
public sealed record BorrowBookCommand(Guid BookId, Guid ReaderId) : ICommand<BorrowedBook>;

