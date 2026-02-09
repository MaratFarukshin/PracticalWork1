using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Application.Commands.Readers;
using PracticalWork.Library.Domain.Exceptions;
using PracticalWork.Library.Domain.Services;
using PracticalWork.Library.Events;

namespace PracticalWork.Library.Application.Handlers.Commands;

/// <summary>
/// Обработчик команды закрытия карточки читателя
/// </summary>
public sealed class CloseReaderCommandHandler : ICommandHandler<CloseReaderCommand>
{
    private readonly IReaderRepository _readerRepository;
    private readonly ReaderValidationService _readerValidationService;
    private readonly IMessagePublisher _messagePublisher;

    public CloseReaderCommandHandler(
        IReaderRepository readerRepository,
        ReaderValidationService readerValidationService,
        IMessagePublisher messagePublisher)
    {
        _readerRepository = readerRepository;
        _readerValidationService = readerValidationService;
        _messagePublisher = messagePublisher;
    }

    public async Task HandleAsync(CloseReaderCommand command, CancellationToken cancellationToken = default)
    {
        var reader = await _readerRepository.GetByIdAsync(command.Id);
        var activeBookIds = await _readerRepository.GetActiveBorrowedBookIdsAsync(command.Id);

        if (!_readerValidationService.CanBeClosed(reader, activeBookIds))
        {
            var booksList = string.Join(", ", activeBookIds);
            throw new ReaderDomainException(
                $"Нельзя закрыть карточку читателя с идентификатором {command.Id}, так как у него есть невозвращенные книги: {booksList}.");
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        await _readerRepository.CloseReaderAsync(command.Id, today);

        // Публикация события ReaderClosedEvent
        var closedReader = await _readerRepository.GetByIdAsync(command.Id);
        var @event = new ReaderClosedEvent
        {
            ReaderId = closedReader.Id,
            FullName = closedReader.FullName,
            ClosedAt = today
        };

        await _messagePublisher.PublishAsync(@event, "reader.closed", cancellationToken);
    }
}

