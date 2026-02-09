using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Services;

public sealed class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly ICacheService _cacheService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IFileValidationService _fileValidationService;
    private const string BookCoversBucket = "book-covers";
    private const long MaxCoverImageSizeInBytes = 5 * 1024 * 1024; // 5 MB

    public BookService(
        IBookRepository bookRepository, 
        ICacheService cacheService,
        IFileStorageService fileStorageService,
        IFileValidationService fileValidationService)
    {
        _bookRepository = bookRepository;
        _cacheService = cacheService;
        _fileStorageService = fileStorageService;
        _fileValidationService = fileValidationService;
    }

    public async Task<Guid> CreateBook(Book book)
    {
        book.Status = BookStatus.Available;
        try
        {
            return await _bookRepository.CreateBook(book);
        }
        catch (Exception ex)
        {
            throw new BookServiceException("Ошибка создание книги!", ex);
        }
    }

    public async Task UpdateBookAsync(Guid id, Book book)
    {
        try
        {
            // 1. Проверка существования книги
            var existingBook = await _bookRepository.GetBookByIdAsync(id);

            // 2. Валидация что книга не в архиве
            if (existingBook.IsArchived)
            {
                throw new BookServiceException($"Книга с идентификатором {id} находится в архиве и не может быть отредактирована.");
            }

            // 3. Обновление разрешенных полей (кроме категории)
            existingBook.Title = book.Title;
            existingBook.Description = book.Description;
            existingBook.Year = book.Year;
            existingBook.Authors = book.Authors;
            // Категория не обновляется согласно требованиям
            // Status может быть обновлен, но не должен быть изменен на Archived через этот метод

            // 4. Сохранение изменений в PostgreSQL
            await _bookRepository.UpdateBookAsync(id, existingBook);

            // 5. Инвалидация кэша связанных данных
            await _cacheService.InvalidateAsync($"book:{id}");
            await _cacheService.InvalidateByPatternAsync("books:*");
        }
        catch (BookServiceException)
        {
            throw;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BookServiceException("Ошибка при обновлении книги!", ex);
        }
    }

    public async Task<Book> ArchiveBookAsync(Guid id)
    {
        try
        {
            // 1. Проверка существования книги
            var book = await _bookRepository.GetBookByIdAsync(id);

            // 2. Проверка что книга не выдана читателю
            if (!book.CanBeArchived())
            {
                throw new BookServiceException($"Книга с идентификатором {id} не может быть заархивирована, так как она выдана читателю.");
            }

            // 3. Перевод статуса книги в "Архив"
            book.Archive();

            // Сохранение изменений в PostgreSQL
            await _bookRepository.UpdateBookAsync(id, book);

            // 4. Инвалидация кэша списков книг
            await _cacheService.InvalidateAsync($"book:{id}");
            await _cacheService.InvalidateByPatternAsync("books:*");

            return book;
        }
        catch (BookServiceException)
        {
            throw;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (InvalidOperationException ex)
        {
            throw new BookServiceException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new BookServiceException("Ошибка при переводе книги в архив!", ex);
        }
    }

    public async Task<PagedResult<Book>> GetBooksAsync(
        BookStatus? status = null,
        BookCategory? category = null,
        string author = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        try
        {
            // 1. Проверка кэша Redis по ключу (фильтры+пагинация)
            var cacheKey = GenerateCacheKey(status, category, author, pageNumber, pageSize);
            var cachedResult = await _cacheService.GetAsync<PagedResult<Book>>(cacheKey);
            
            if (cachedResult != null)
            {
                return cachedResult;
            }

            // 2. При отсутствии в кэше запрос к PostgreSQL
            // 3. Фильтрация по статусу, категории, автору
            // 4. Применение пагинации
            var result = await _bookRepository.GetBooksAsync(status, category, author, pageNumber, pageSize);

            // 5. Сохранение результата в кэш на 10 минут
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }
        catch (Exception ex)
        {
            throw new BookServiceException("Ошибка при получении списка книг!", ex);
        }
    }

    private static string GenerateCacheKey(BookStatus? status, BookCategory? category, string author, int pageNumber, int pageSize)
    {
        var parts = new List<string> { "books" };
        
        if (status.HasValue)
            parts.Add($"status:{status.Value}");
        
        if (category.HasValue)
            parts.Add($"category:{category.Value}");
        
        if (!string.IsNullOrWhiteSpace(author))
            parts.Add($"author:{author.ToLowerInvariant()}");
        
        parts.Add($"page:{pageNumber}");
        parts.Add($"size:{pageSize}");
        
        return string.Join(":", parts);
    }

    public async Task<string> AddBookDetailsAsync(Guid id, string description, Stream coverImageStream, string fileName, string contentType)
    {
        try
        {
            // 1. Проверка существования книги
            var book = await _bookRepository.GetBookByIdAsync(id);

            // 2. Валидация файла обложки (формат, размер)
            var validationResult = _fileValidationService.ValidateCoverImage(coverImageStream, fileName, contentType, MaxCoverImageSizeInBytes);
            if (!validationResult.IsValid)
            {
                throw new BookServiceException(validationResult.ErrorMessage);
            }

            // 3. Загрузка обложки в MinIO
            var objectName = $"{id}/{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            var coverImagePath = await _fileStorageService.UploadFileAsync(BookCoversBucket, objectName, coverImageStream, contentType);

            // 4. Обновление описания и пути к обложке
            book.UpdateDetails(description, coverImagePath);
            await _bookRepository.UpdateBookAsync(id, book);

            // 5. Инвалидация кэша деталей книги
            await _cacheService.InvalidateAsync($"book:{id}");
            await _cacheService.InvalidateByPatternAsync("books:*");

            return coverImagePath;
        }
        catch (BookServiceException)
        {
            throw;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BookServiceException("Ошибка при добавлении деталей книги!", ex);
        }
    }
}