using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Data.PostgreSql.Repositories;

public sealed class BookRepository : IBookRepository
{
    private readonly AppDbContext _appDbContext;

    public BookRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<Guid> CreateBook(Book book)
    {
        AbstractBookEntity entity = book.Category switch
        {
            BookCategory.ScientificBook => new ScientificBookEntity(),
            BookCategory.EducationalBook => new EducationalBookEntity(),
            BookCategory.FictionBook => new FictionBookEntity(),
            _ => throw new ArgumentException($"Неподдерживаемая категория книги: {book.Category}", nameof(book.Category))
        };

        entity.Title = book.Title;
        entity.Description = book.Description;
        entity.Year = book.Year;
        entity.Authors = book.Authors;
        entity.Status = book.Status;

        _appDbContext.Add(entity);
        await _appDbContext.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<Book> GetBookByIdAsync(Guid id)
    {
        var entity = await _appDbContext.Books
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            throw new ArgumentException($"Книга с идентификатором {id} не найдена.", nameof(id));

        return MapToBook(entity);
    }

    public async Task UpdateBookAsync(Guid id, Book book)
    {
        var entity = await _appDbContext.Books
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            throw new ArgumentException($"Книга с идентификатором {id} не найдена.", nameof(id));

        // Обновляем разрешенные поля (кроме категории)
        entity.Title = book.Title;
        entity.Description = book.Description;
        entity.Year = book.Year;
        entity.Authors = book.Authors;
        entity.Status = book.Status;
        entity.CoverImagePath = book.CoverImagePath;

        await _appDbContext.SaveChangesAsync();
    }

    private static BookCategory GetCategoryFromEntity(AbstractBookEntity entity)
    {
        return entity switch
        {
            ScientificBookEntity => BookCategory.ScientificBook,
            EducationalBookEntity => BookCategory.EducationalBook,
            FictionBookEntity => BookCategory.FictionBook,
            _ => BookCategory.Default
        };
    }

    public async Task<PagedResult<Book>> GetBooksAsync(
        BookStatus? status = null,
        BookCategory? category = null,
        string author = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var query = _appDbContext.Books.AsQueryable();

        // Фильтрация по статусу
        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        // Фильтрация по категории
        if (category.HasValue)
        {
            query = category.Value switch
            {
                BookCategory.ScientificBook => query.OfType<ScientificBookEntity>(),
                BookCategory.EducationalBook => query.OfType<EducationalBookEntity>(),
                BookCategory.FictionBook => query.OfType<FictionBookEntity>(),
                _ => query
            };
        }

        // Фильтрация по автору
        if (!string.IsNullOrWhiteSpace(author))
        {
            query = query.Where(x => x.Authors != null && x.Authors.Any(a => a.Contains(author, StringComparison.OrdinalIgnoreCase)));
        }

        // Подсчет общего количества
        var totalCount = await query.CountAsync();

        // Применение пагинации
        var skip = (pageNumber - 1) * pageSize;
        var entities = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        var books = entities.Select(MapToBook).ToList();

        return new PagedResult<Book>
        {
            Items = books,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<BookWithBorrowInfo>> GetLibraryBooksAsync(
        BookCategory? category = null,
        string author = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        // 2. Запрос не архивных книг из PostgreSQL
        var query = from book in _appDbContext.Books
                    where book.Status != BookStatus.Archived
                    // 3. Обогащение данных информацией о выдаче
                    join borrow in _appDbContext.BookBorrows
                        on new { BookId = book.Id, Status = BookIssueStatus.Issued }
                        equals new { BookId = borrow.BookId, Status = borrow.Status }
                        into borrows
                    from borrow in borrows.DefaultIfEmpty()
                    select new { book, borrow };

        // Фильтрация по категории
        if (category.HasValue)
        {
            query = category.Value switch
            {
                BookCategory.ScientificBook => query.Where(x => x.book is ScientificBookEntity),
                BookCategory.EducationalBook => query.Where(x => x.book is EducationalBookEntity),
                BookCategory.FictionBook => query.Where(x => x.book is FictionBookEntity),
                _ => query
            };
        }

        // Фильтрация по автору
        if (!string.IsNullOrWhiteSpace(author))
        {
            query = query.Where(x => x.book.Authors != null && x.book.Authors.Any(a => a.Contains(author, StringComparison.OrdinalIgnoreCase)));
        }

        // Подсчет общего количества (группируем по книге, чтобы избежать дубликатов из-за JOIN)
        var totalCount = await query
            .Select(x => x.book.Id)
            .Distinct()
            .CountAsync();

        // 4. Применение фильтров и пагинации
        var skip = (pageNumber - 1) * pageSize;
        var data = await query
            .Select(x => x.book.Id)
            .Distinct()
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        // Получаем полные данные книг и их выдач
        var booksWithBorrows = await (from book in _appDbContext.Books
                                      where data.Contains(book.Id)
                                      join borrow in _appDbContext.BookBorrows
                                          on new { BookId = book.Id, Status = BookIssueStatus.Issued }
                                          equals new { BookId = borrow.BookId, Status = borrow.Status }
                                          into borrows
                                      from borrow in borrows.DefaultIfEmpty()
                                      select new { book, borrow })
            .ToListAsync();

        var books = booksWithBorrows
            .GroupBy(x => x.book.Id)
            .Select(g => MapToBookWithBorrowInfo(g.First().book, g.First().borrow))
            .ToList();

        return new PagedResult<BookWithBorrowInfo>
        {
            Items = books,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    private static Book MapToBook(AbstractBookEntity entity)
    {
        return new Book
        {
            Id = entity.Id,
            Title = entity.Title,
            Authors = entity.Authors,
            Description = entity.Description,
            Year = entity.Year,
            Category = GetCategoryFromEntity(entity),
            Status = entity.Status,
            CoverImagePath = entity.CoverImagePath,
            IsArchived = entity.Status == BookStatus.Archived,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public async Task<Book> FindBookByIdOrTitleAsync(string idOrTitle)
    {
        AbstractBookEntity entity = null;

        // Попытка распарсить как Guid
        if (Guid.TryParse(idOrTitle, out var bookId))
        {
            // Поиск по ID
            entity = await _appDbContext.Books
                .FirstOrDefaultAsync(x => x.Id == bookId);
        }
        else
        {
            // Поиск по названию
            entity = await _appDbContext.Books
                .FirstOrDefaultAsync(x => x.Title == idOrTitle);
        }

        if (entity == null)
        {
            throw new ArgumentException($"Книга с идентификатором или названием '{idOrTitle}' не найдена.", nameof(idOrTitle));
        }

        return MapToBook(entity);
    }

    private static BookWithBorrowInfo MapToBookWithBorrowInfo(AbstractBookEntity entity, BookBorrowEntity borrow)
    {
        return new BookWithBorrowInfo
        {
            Id = entity.Id,
            Title = entity.Title,
            Authors = entity.Authors,
            Description = entity.Description,
            Year = entity.Year,
            Category = GetCategoryFromEntity(entity),
            Status = entity.Status,
            CoverImagePath = entity.CoverImagePath,
            IsArchived = entity.Status == BookStatus.Archived,
            ReaderId = borrow?.ReaderId,
            BorrowDate = borrow?.BorrowDate,
            DueDate = borrow?.DueDate
        };
    }
}