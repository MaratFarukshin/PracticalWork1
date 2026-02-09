using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Services;

public sealed class ReaderService : IReaderService
{
    private readonly IReaderRepository _readerRepository;

    public ReaderService(IReaderRepository readerRepository)
    {
        _readerRepository = readerRepository;
    }

    public async Task<Guid> CreateReader(Reader reader)
    {
        try
        {
            // 1. Валидация данных (ФИО, телефон, дата окончания) - выполняется через FluentValidation

            // 2. Проверка уникальности номера телефона
            var phoneExists = await _readerRepository.ExistsByPhoneNumberAsync(reader.PhoneNumber);
            if (phoneExists)
            {
                throw new ReaderServiceException($"Читатель с номером телефона {reader.PhoneNumber} уже существует.");
            }

            // 3. Создание активной карточки читателя
            reader.IsActive = true;

            // 4. Сохранение в PostgreSQL
            return await _readerRepository.CreateReader(reader);
        }
        catch (ReaderServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ReaderServiceException("Ошибка при создании карточки читателя!", ex);
        }
    }

    public async Task<DateOnly> ExtendReaderAsync(Guid id, DateOnly newExpiryDate)
    {
        try
        {
            // 1. Проверка существования и активности карточки
            var reader = await _readerRepository.GetByIdAsync(id);

            if (!reader.IsActive)
            {
                throw new ReaderServiceException($"Карточка читателя с идентификатором {id} неактивна и не может быть продлена.");
            }

            // 2. Валидация новой даты окончания
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (newExpiryDate <= today)
            {
                throw new ReaderServiceException("Новая дата окончания действия карточки должна быть в будущем.");
            }

            if (newExpiryDate <= reader.ExpiryDate)
            {
                throw new ReaderServiceException("Новая дата окончания действия карточки должна быть позже текущей даты окончания.");
            }

            // 3. Обновление даты окончания действия
            await _readerRepository.UpdateExpiryDateAsync(id, newExpiryDate);

            // 4. Возврат новой даты
            return newExpiryDate;
        }
        catch (ReaderServiceException)
        {
            throw;
        }
        catch (ArgumentException ex)
        {
            throw new ReaderServiceException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new ReaderServiceException("Ошибка при продлении срока действия карточки читателя!", ex);
        }
    }

    public async Task CloseReaderAsync(Guid id)
    {
        try
        {
            // 1. Проверка существования карточки
            var reader = await _readerRepository.GetByIdAsync(id);

            // 2. Проверка что у читателя нет взятых книг
            var activeBookIds = await _readerRepository.GetActiveBorrowedBookIdsAsync(id);
            if (activeBookIds.Count > 0)
            {
                var booksList = string.Join(", ", activeBookIds);
                throw new ReaderServiceException(
                    $"Нельзя закрыть карточку читателя с идентификатором {id}, так как у него есть невозвращенные книги: {booksList}.");
            }

            // 5. Установка даты окончания на текущую дату
            var today = DateOnly.FromDateTime(DateTime.Today);

            // 4. Перевод карточки в неактивное состояние и обновление даты окончания
            await _readerRepository.CloseReaderAsync(id, today);
        }
        catch (ReaderServiceException)
        {
            throw;
        }
        catch (ArgumentException ex)
        {
            throw new ReaderServiceException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new ReaderServiceException("Ошибка при закрытии карточки читателя!", ex);
        }
    }

    public async Task<IReadOnlyList<BorrowedBook>> GetBorrowedBooksAsync(Guid id)
    {
        try
        {
            // 1. Проверка существования карточки
            var reader = await _readerRepository.GetByIdAsync(id);

            // 2-4. Получение активных выдач и информации о книгах (делается в репозитории)
            return await _readerRepository.GetActiveBorrowedBooksAsync(id);
        }
        catch (ReaderServiceException)
        {
            throw;
        }
        catch (ArgumentException ex)
        {
            throw new ReaderServiceException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new ReaderServiceException("Ошибка при получении списка взятых книг читателя!", ex);
        }
    }
}

