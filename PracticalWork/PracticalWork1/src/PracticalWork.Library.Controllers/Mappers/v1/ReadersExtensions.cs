using PracticalWork.Library.Contracts.v1.Readers.Request;
using PracticalWork.Library.Contracts.v1.Readers.Response;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Controllers.Mappers.v1;

public static class ReadersExtensions
{
    public static Reader ToReader(this CreateReaderRequest request) =>
        new()
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            ExpiryDate = request.ExpiryDate,
            IsActive = false // Будет установлено в true в сервисе
        };

    public static ReaderBorrowedBookResponse ToReaderBorrowedBookResponse(this BorrowedBook book) =>
        new(
            book.BookId,
            book.Title,
            book.Authors,
            book.BorrowDate,
            book.DueDate
        );

    public static GetReaderBorrowedBooksResponse ToGetReaderBorrowedBooksResponse(
        this IReadOnlyList<BorrowedBook> borrowedBooks,
        Guid readerId) =>
        new(
            readerId,
            borrowedBooks.Select(b => b.ToReaderBorrowedBookResponse()).ToList()
        );
}

