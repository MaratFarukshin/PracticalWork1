using PracticalWork.Library.Contracts.v1.Library.Response;
using ContractsEnums = PracticalWork.Library.Contracts.v1.Enums;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Controllers.Mappers.v1;

public static class LibraryExtensions
{
    public static BorrowBookResponse ToBorrowBookResponse(this BorrowedBook book, Guid readerId) =>
        new(
            book.BookId,
            readerId,
            book.BorrowDate,
            book.DueDate
        );

    public static LibraryBookListItemResponse ToLibraryBookListItemResponse(this BookWithBorrowInfo book) =>
        new(
            book.Id,
            book.Title,
            (PracticalWork.Library.Contracts.v1.Enums.BookCategory)book.Category,
            book.Authors,
            (PracticalWork.Library.Contracts.v1.Enums.BookStatus)book.Status,
            book.IsArchived,
            book.ReaderId,
            book.BorrowDate,
            book.DueDate
        );

    public static GetLibraryBooksResponse ToGetLibraryBooksResponse(this PagedResult<BookWithBorrowInfo> pagedResult) =>
        new(
            pagedResult.Items.Select(book => book.ToLibraryBookListItemResponse()).ToList(),
            pagedResult.TotalCount,
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalPages
        );

    public static ReturnBookResponse ToReturnBookResponse(this ReturnedBook returnedBook) =>
        new(
            returnedBook.BookId,
            returnedBook.ReaderId,
            returnedBook.ReturnDate,
            returnedBook.WasOverdue
        );

    public static GetLibraryBookDetailsResponse ToGetLibraryBookDetailsResponse(this BookDetails bookDetails) =>
        new(
            bookDetails.Id,
            bookDetails.Title,
            (PracticalWork.Library.Contracts.v1.Enums.BookCategory)bookDetails.Category,
            bookDetails.Authors,
            bookDetails.Description,
            bookDetails.Year,
            bookDetails.CoverImageUrl,
            (PracticalWork.Library.Contracts.v1.Enums.BookStatus)bookDetails.Status,
            bookDetails.IsArchived
        );
}