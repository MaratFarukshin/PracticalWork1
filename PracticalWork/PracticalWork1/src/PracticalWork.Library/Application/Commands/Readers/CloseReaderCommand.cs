using PracticalWork.Library.Application.Abstractions;

namespace PracticalWork.Library.Application.Commands.Readers;

/// <summary>
/// Команда закрытия карточки читателя
/// </summary>
public sealed record CloseReaderCommand(Guid Id) : ICommand;

