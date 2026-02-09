using PracticalWork.Library.Application.Abstractions;

namespace PracticalWork.Library.Application.Commands.Books;

/// <summary>
/// Команда добавления деталей книги
/// </summary>
public sealed record AddBookDetailsCommand(
    Guid Id,
    string Description,
    Stream CoverImageStream,
    string FileName,
    string ContentType
) : ICommand<string>;

