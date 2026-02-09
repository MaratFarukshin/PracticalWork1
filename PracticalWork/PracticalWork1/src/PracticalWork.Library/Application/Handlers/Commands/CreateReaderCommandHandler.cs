using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Application.Commands.Readers;
using PracticalWork.Library.Domain.Exceptions;
using PracticalWork.Library.Events;

namespace PracticalWork.Library.Application.Handlers.Commands;

/// <summary>
/// Обработчик команды создания читателя
/// </summary>
public sealed class CreateReaderCommandHandler : ICommandHandler<CreateReaderCommand, Guid>
{
    private readonly IReaderRepository _readerRepository;
    private readonly IMessagePublisher _messagePublisher;

    public CreateReaderCommandHandler(
        IReaderRepository readerRepository,
        IMessagePublisher messagePublisher)
    {
        _readerRepository = readerRepository;
        _messagePublisher = messagePublisher;
    }

    public async Task<Guid> HandleAsync(CreateReaderCommand command, CancellationToken cancellationToken = default)
    {
        var phoneExists = await _readerRepository.ExistsByPhoneNumberAsync(command.Reader.PhoneNumber);
        if (phoneExists)
        {
            throw new ReaderDomainException($"Читатель с номером телефона {command.Reader.PhoneNumber} уже существует.");
        }

        command.Reader.IsActive = true;
        var readerId = await _readerRepository.CreateReader(command.Reader);

        // Публикация события ReaderCreatedEvent
        var reader = await _readerRepository.GetByIdAsync(readerId);
        var @event = new ReaderCreatedEvent
        {
            ReaderId = reader.Id,
            FullName = reader.FullName,
            PhoneNumber = reader.PhoneNumber,
            ExpiryDate = reader.ExpiryDate,
            CreatedAt = reader.CreatedAt
        };

        await _messagePublisher.PublishAsync(@event, "reader.created", cancellationToken);

        return readerId;
    }
}

