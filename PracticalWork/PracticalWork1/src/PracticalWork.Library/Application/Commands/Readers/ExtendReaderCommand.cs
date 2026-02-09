using PracticalWork.Library.Application.Abstractions;

namespace PracticalWork.Library.Application.Commands.Readers;

/// <summary>
/// Команда продления срока действия карточки читателя
/// </summary>
public sealed record ExtendReaderCommand(Guid Id, DateOnly NewExpiryDate) : ICommand<DateOnly>;

