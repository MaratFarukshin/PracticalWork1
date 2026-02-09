using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Application.Commands.Library;
using PracticalWork.Library.Application.Queries.Library;
using PracticalWork.Library.Contracts.v1.Library.Request;
using PracticalWork.Library.Contracts.v1.Library.Response;
using PracticalWork.Library.Controllers.Mappers.v1;
using PracticalWork.Library.Enums;

namespace PracticalWork.Library.Controllers.Api.v1;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/library")]
public class LibraryController : Controller
{
    private readonly IMediator _mediator;

    public LibraryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary> Выдача книги читателю</summary>
    [HttpPost("borrow")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BorrowBookResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> BorrowBook(BorrowBookRequest request)
    {
        var command = new BorrowBookCommand(request.BookId, request.ReaderId);
        var borrowedBook = await _mediator.SendAsync(command);

        return Ok(borrowedBook.ToBorrowBookResponse(request.ReaderId));
    }

    /// <summary> Получение списка книг библиотеки</summary>
    [HttpGet("books")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GetLibraryBooksResponse), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetLibraryBooks([FromQuery] GetLibraryBooksRequest request)
    {
        var query = new GetLibraryBooksQuery(
            request.Category != null ? (BookCategory)request.Category : null,
            request.Author,
            request.PageNumber,
            request.PageSize);

        var books = await _mediator.SendAsync(query);

        return Ok(books.ToGetLibraryBooksResponse());
    }

    /// <summary> Возврат книги читателем</summary>
    [HttpPost("return")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ReturnBookResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ReturnBook(ReturnBookRequest request)
    {
        var command = new ReturnBookCommand(request.BookId, request.ReaderId);
        var returnedBook = await _mediator.SendAsync(command);

        return Ok(returnedBook.ToReturnBookResponse());
    }

    /// <summary> Получение деталей книги</summary>
    [HttpGet("{idOrTitle}/details")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GetLibraryBookDetailsResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetLibraryBookDetails(string idOrTitle)
    {
        var query = new GetLibraryBookDetailsQuery(idOrTitle);
        var bookDetails = await _mediator.SendAsync(query);

        return Ok(bookDetails.ToGetLibraryBookDetailsResponse());
    }
}