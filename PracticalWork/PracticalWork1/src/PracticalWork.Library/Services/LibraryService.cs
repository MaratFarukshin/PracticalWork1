using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Services;

/// <summary>
/// Сервис для операций библиотеки (выдача книг)
/// </summary>
public sealed class LibraryService : ILibraryService
{
    private readonly IBookRepository _bookRepository;
    private readonly IReaderRepository _readerRepository;
    private readonly IBookBorrowRepository _bookBorrowRepository;
    private readonly ICacheService _cacheService;
    private readonly IFileStorageService _fileStorageService;

    public LibraryService(
        IBookRepository bookRepository,
        IReaderRepository readerRepository,
        IBookBorrowRepository bookBorrowRepository,
        ICacheService cacheService,
        IFileStorageService fileStorageService)
    {
        _bookRepository = bookRepository;
        _readerRepository = readerRepository;
        _bookBorrowRepository = bookBorrowRepository;
        _cacheService = cacheService;
        _fileStorageService = fileStorageService;
    }

    public async Task<BorrowedBook> BorrowBookAsync(Guid bookId, Guid readerId)
    {
        try
        {
            // 1. Проверка существования книги и читателя
            var book = await _bookRepository.GetBookByIdAsync(bookId);
            var reader = await _readerRepository.GetByIdAsync(readerId);

            // 2. Проверка доступности книги (не архив, не выдана)
            if (!book.CanBeBorrowed())
            {
                throw new LibraryServiceException($"Книга с идентификатором {bookId} недоступна для выдачи.");
            }

            // 3. Проверка активности карточки читателя
            if (!reader.IsActive)
            {
                throw new LibraryServiceException($"Карточка читателя с идентификатором {readerId} неактивна.");
            }

            // 4. Создание записи о выдаче
            var borrowDate = DateOnly.FromDateTime(DateTime.Today);
            // 5. Установка срока возврата (текущая дата + 30 дней)
            var dueDate = borrowDate.AddDays(30);

            await _bookBorrowRepository.CreateBorrowAsync(bookId, readerId, borrowDate, dueDate);

            // 6. Обновление статуса книги
            book.Status = BookStatus.Borrow;
            await _bookRepository.UpdateBookAsync(bookId, book);

            // Результат для ответа
            return new BorrowedBook
            {
                BookId = book.Id,
                Title = book.Title,
                Authors = book.Authors,
                BorrowDate = borrowDate,
                DueDate = dueDate
            };
        }
        catch (LibraryServiceException)
        {
            throw;
        }
        catch (ArgumentException ex)
        {
            throw new LibraryServiceException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new LibraryServiceException("Ошибка при выдаче книги читателю!", ex);
        }
    }

    public async Task<PagedResult<BookWithBorrowInfo>> GetLibraryBooksAsync(
        BookCategory? category = null,
        string author = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        try
        {
            // 1. Проверка кэша Redis по ключу
            var cacheKey = GenerateLibraryBooksCacheKey(category, author, pageNumber, pageSize);
            var cachedResult = await _cacheService.GetAsync<PagedResult<BookWithBorrowInfo>>(cacheKey);
            
            if (cachedResult != null)
            {
                return cachedResult;
            }

            // 2. Запрос не архивных книг из PostgreSQL
            // 3. Обогащение данных информацией о выдаче
            // 4. Применение фильтров и пагинации
            var result = await _bookRepository.GetLibraryBooksAsync(category, author, pageNumber, pageSize);

            // 5. Сохранение в кэш на 5 минут
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }
        catch (Exception ex)
        {
            throw new LibraryServiceException("Ошибка при получении списка книг библиотеки!", ex);
        }
    }

    public async Task<ReturnedBook> ReturnBookAsync(Guid bookId, Guid readerId)
    {
        try
        {
            // 1. Поиск активной записи о выдаче
            var borrowId = await _bookBorrowRepository.FindActiveBorrowAsync(bookId, readerId);
            
            if (!borrowId.HasValue)
            {
                throw new LibraryServiceException($"Активная запись о выдаче книги {bookId} читателю {readerId} не найдена.");
            }

            // 2. Проверка что книга действительно выдана
            var book = await _bookRepository.GetBookByIdAsync(bookId);
            
            if (book.Status != BookStatus.Borrow)
            {
                throw new LibraryServiceException($"Книга с идентификатором {bookId} не находится в статусе 'Выдана'.");
            }

            // 3. Обновление даты возврата и статуса
            var returnDate = DateOnly.FromDateTime(DateTime.Today);
            
            // Получаем информацию о выдаче для проверки просрочки
            var dueDate = await _bookBorrowRepository.GetDueDateAsync(borrowId.Value);
            
            if (!dueDate.HasValue)
            {
                throw new LibraryServiceException($"Не удалось получить информацию о сроке возврата для записи {borrowId.Value}.");
            }

            var wasOverdue = returnDate > dueDate.Value;
            var status = wasOverdue ? BookIssueStatus.Overdue : BookIssueStatus.Returned;

            await _bookBorrowRepository.UpdateBorrowAsync(borrowId.Value, returnDate, status);

            // 4. Обновление статуса книги на "Доступна"
            book.Status = BookStatus.Available;
            await _bookRepository.UpdateBookAsync(bookId, book);

            // Инвалидация кэша
            await _cacheService.InvalidateByPatternAsync("library:books:*");

            return new ReturnedBook
            {
                BookId = bookId,
                ReaderId = readerId,
                ReturnDate = returnDate,
                WasOverdue = wasOverdue
            };
        }
        catch (LibraryServiceException)
        {
            throw;
        }
        catch (ArgumentException ex)
        {
            throw new LibraryServiceException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new LibraryServiceException("Ошибка при возврате книги!", ex);
        }
    }

    public async Task<BookDetails> GetLibraryBookDetailsAsync(string idOrTitle)
    {
        try
        {
            // 1. Поиск книги по ID или названию
            var book = await _bookRepository.FindBookByIdOrTitleAsync(idOrTitle);

            // 2. Получение описания и пути к обложке
            // (уже есть в объекте book)

            // 3. Формирование URL для доступа к обложке
            var coverImageUrl = _fileStorageService.GetFileUrl(book.CoverImagePath);

            // 4. Возврат детальной информации
            return new BookDetails
            {
                Id = book.Id,
                Title = book.Title,
                Category = book.Category,
                Authors = book.Authors,
                Description = book.Description,
                Year = book.Year,
                CoverImageUrl = coverImageUrl,
                Status = book.Status,
                IsArchived = book.IsArchived
            };
        }
        catch (ArgumentException ex)
        {
            throw new LibraryServiceException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new LibraryServiceException("Ошибка при получении детальной информации о книге!", ex);
        }
    }

    private static string GenerateLibraryBooksCacheKey(BookCategory? category, string author, int pageNumber, int pageSize)
    {
        var parts = new List<string> { "library:books" };
        
        if (category.HasValue)
            parts.Add($"category:{category.Value}");
        
        if (!string.IsNullOrWhiteSpace(author))
            parts.Add($"author:{author.ToLowerInvariant()}");
        
        parts.Add($"page:{pageNumber}");
        parts.Add($"size:{pageSize}");
        
        return string.Join(":", parts);
    }
}