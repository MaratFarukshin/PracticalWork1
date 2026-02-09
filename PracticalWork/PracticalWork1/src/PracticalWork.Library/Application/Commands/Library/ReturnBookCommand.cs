using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Application.Commands.Library;

/// <summary>
/// Команда возврата книги
/// </summary>
public sealed record ReturnBookCommand(Guid BookId, Guid ReaderId) : ICommand<ReturnedBook>;

