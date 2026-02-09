using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Data.PostgreSql.Repositories;

public sealed class ReaderRepository : IReaderRepository
{
    private readonly AppDbContext _appDbContext;

    public ReaderRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<Guid> CreateReader(Reader reader)
    {
        var entity = new ReaderEntity
        {
            FullName = reader.FullName,
            PhoneNumber = reader.PhoneNumber,
            ExpiryDate = reader.ExpiryDate,
            IsActive = reader.IsActive
        };

        _appDbContext.Readers.Add(entity);
        await _appDbContext.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber)
    {
        return await _appDbContext.Readers
            .AnyAsync(r => r.PhoneNumber == phoneNumber);
    }

    public async Task<Reader> GetByIdAsync(Guid id)
    {
        var entity = await _appDbContext.Readers
            .FirstOrDefaultAsync(r => r.Id == id);

        if (entity == null)
            throw new ArgumentException($"Читатель с идентификатором {id} не найден.", nameof(id));

        return new Reader
        {
            Id = entity.Id,
            FullName = entity.FullName,
            PhoneNumber = entity.PhoneNumber,
            ExpiryDate = entity.ExpiryDate,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public async Task UpdateExpiryDateAsync(Guid id, DateOnly newExpiryDate)
    {
        var entity = await _appDbContext.Readers
            .FirstOrDefaultAsync(r => r.Id == id);

        if (entity == null)
            throw new ArgumentException($"Читатель с идентификатором {id} не найден.", nameof(id));

        entity.ExpiryDate = newExpiryDate;

        await _appDbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Guid>> GetActiveBorrowedBookIdsAsync(Guid readerId)
    {
        return await _appDbContext.BookBorrows
            .Where(b => b.ReaderId == readerId && b.Status == Enums.BookIssueStatus.Issued)
            .Select(b => b.BookId)
            .ToListAsync();
    }

    public async Task CloseReaderAsync(Guid id, DateOnly closeDate)
    {
        var entity = await _appDbContext.Readers
            .FirstOrDefaultAsync(r => r.Id == id);

        if (entity == null)
            throw new ArgumentException($"Читатель с идентификатором {id} не найден.", nameof(id));

        entity.IsActive = false;
        entity.ExpiryDate = closeDate;

        await _appDbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<BorrowedBook>> GetActiveBorrowedBooksAsync(Guid readerId)
    {
        var query = from borrow in _appDbContext.BookBorrows
                    join book in _appDbContext.Books on borrow.BookId equals book.Id
                    where borrow.ReaderId == readerId && borrow.Status == Enums.BookIssueStatus.Issued
                    select new { borrow, book };

        var data = await query.ToListAsync();

        return data
            .Select(x => new BorrowedBook
            {
                BookId = x.book.Id,
                Title = x.book.Title,
                Authors = x.book.Authors,
                BorrowDate = x.borrow.BorrowDate,
                DueDate = x.borrow.DueDate
            })
            .ToList();
    }
}

