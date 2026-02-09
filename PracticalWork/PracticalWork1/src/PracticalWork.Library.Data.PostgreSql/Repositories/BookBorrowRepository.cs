using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;
using PracticalWork.Library.Enums;

namespace PracticalWork.Library.Data.PostgreSql.Repositories;

/// <summary>
/// Репозиторий для работы с выдачами книг
/// </summary>
public sealed class BookBorrowRepository : IBookBorrowRepository
{
    private readonly AppDbContext _appDbContext;

    public BookBorrowRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<Guid> CreateBorrowAsync(Guid bookId, Guid readerId, DateOnly borrowDate, DateOnly dueDate)
    {
        var entity = new BookBorrowEntity
        {
            BookId = bookId,
            ReaderId = readerId,
            BorrowDate = borrowDate,
            DueDate = dueDate,
            Status = BookIssueStatus.Issued
        };

        _appDbContext.BookBorrows.Add(entity);
        await _appDbContext.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<Guid?> FindActiveBorrowAsync(Guid bookId, Guid readerId)
    {
        var borrow = await _appDbContext.BookBorrows
            .FirstOrDefaultAsync(b => 
                b.BookId == bookId && 
                b.ReaderId == readerId && 
                b.Status == BookIssueStatus.Issued);

        return borrow?.Id;
    }

    public async Task<DateOnly?> GetDueDateAsync(Guid borrowId)
    {
        var borrow = await _appDbContext.BookBorrows
            .FirstOrDefaultAsync(b => b.Id == borrowId);

        return borrow?.DueDate;
    }

    public async Task UpdateBorrowAsync(Guid borrowId, DateOnly returnDate, BookIssueStatus status)
    {
        var borrow = await _appDbContext.BookBorrows
            .FirstOrDefaultAsync(b => b.Id == borrowId);

        if (borrow == null)
            throw new ArgumentException($"Запись о выдаче с идентификатором {borrowId} не найдена.", nameof(borrowId));

        borrow.ReturnDate = returnDate;
        borrow.Status = status;

        await _appDbContext.SaveChangesAsync();
    }
}