using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Application.Commands.Readers;
using PracticalWork.Library.Domain.Exceptions;
using PracticalWork.Library.Domain.Services;

namespace PracticalWork.Library.Application.Handlers.Commands;

/// <summary>
/// Обработчик команды продления срока действия карточки читателя
/// </summary>
public sealed class ExtendReaderCommandHandler : ICommandHandler<ExtendReaderCommand, DateOnly>
{
    private readonly IReaderRepository _readerRepository;
    private readonly ReaderValidationService _readerValidationService;

    public ExtendReaderCommandHandler(
        IReaderRepository readerRepository,
        ReaderValidationService readerValidationService)
    {
        _readerRepository = readerRepository;
        _readerValidationService = readerValidationService;
    }

    public async Task<DateOnly> HandleAsync(ExtendReaderCommand command, CancellationToken cancellationToken = default)
    {
        var reader = await _readerRepository.GetByIdAsync(command.Id);

        if (!_readerValidationService.CanBeExtended(reader, command.NewExpiryDate))
        {
            if (!reader.IsActive)
            {
                throw new ReaderDomainException($"Карточка читателя с идентификатором {command.Id} неактивна и не может быть продлена.");
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            if (command.NewExpiryDate <= today)
            {
                throw new ReaderDomainException("Новая дата окончания действия карточки должна быть в будущем.");
            }

            if (command.NewExpiryDate <= reader.ExpiryDate)
            {
                throw new ReaderDomainException("Новая дата окончания действия карточки должна быть позже текущей даты окончания.");
            }
        }

        await _readerRepository.UpdateExpiryDateAsync(command.Id, command.NewExpiryDate);
        return command.NewExpiryDate;
    }
}

