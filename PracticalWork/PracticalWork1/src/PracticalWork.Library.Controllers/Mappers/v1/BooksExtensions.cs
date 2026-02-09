using PracticalWork.Library.Contracts.v1.Books.Request;
using PracticalWork.Library.Contracts.v1.Books.Response;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Controllers.Mappers.v1;

public static class BooksExtensions
{
    public static Book ToBook(this CreateBookRequest request) =>
        new()
        {
            Authors = request.Authors,
            Title = request.Title,
            Description = request.Description,
            Year = request.Year,
            Category = (BookCategory)request.Category
        };

    public static Book ToBook(this UpdateBookRequest request) =>
        new()
        {
            Authors = request.Authors,
            Title = request.Title,
            Description = request.Description,
            Year = request.Year
            // Category не устанавливается, так как она не может быть изменена при обновлении
        };

    public static BookListItemResponse ToListItemResponse(this Book book) =>
        new(
            book.Id,
            book.Title,
            (PracticalWork.Library.Contracts.v1.Enums.BookCategory)book.Category,
            book.Authors,
            (PracticalWork.Library.Contracts.v1.Enums.BookStatus)book.Status,
            book.IsArchived
        );

    public static GetBooksResponse ToGetBooksResponse(this PagedResult<Book> pagedResult) =>
        new(
            pagedResult.Items.Select(book => book.ToListItemResponse()).ToList(),
            pagedResult.TotalCount,
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalPages
        );
}