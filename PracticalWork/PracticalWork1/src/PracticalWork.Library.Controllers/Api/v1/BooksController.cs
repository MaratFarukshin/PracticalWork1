using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Application.Commands.Books;
using PracticalWork.Library.Application.Queries.Books;
using PracticalWork.Library.Contracts.v1.Books.Request;
using PracticalWork.Library.Contracts.v1.Books.Response;
using PracticalWork.Library.Controllers.Mappers.v1;

namespace PracticalWork.Library.Controllers.Api.v1;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/books")]
public class BooksController : Controller
{
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary> Создание новой книги</summary>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CreateBookResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CreateBook(CreateBookRequest request)
    {
        var command = new CreateBookCommand(request.ToBook());
        var bookId = await _mediator.SendAsync(command);

        return Ok(new CreateBookResponse(bookId));
    }

    /// <summary> Редактирование книги</summary>
    [HttpPut("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(UpdateBookResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateBook(Guid id, UpdateBookRequest request)
    {
        var command = new UpdateBookCommand(id, request.ToBook());
        await _mediator.SendAsync(command);

        return Ok(new UpdateBookResponse(id));
    }

    /// <summary> Перевод книги в архив</summary>
    [HttpPost("{id}/archive")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ArchiveBookResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ArchiveBook(Guid id)
    {
        var command = new ArchiveBookCommand(id);
        var book = await _mediator.SendAsync(command);

        return Ok(new ArchiveBookResponse(
            id,
            book.Title,
            DateTime.UtcNow
        ));
    }

    /// <summary> Получение списка книг</summary>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GetBooksResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetBooks([FromQuery] GetBooksRequest request)
    {
        var query = new GetBooksQuery(
            request.Status.HasValue ? (PracticalWork.Library.Enums.BookStatus?)request.Status.Value : null,
            request.Category.HasValue ? (PracticalWork.Library.Enums.BookCategory?)request.Category.Value : null,
            request.Author,
            request.PageNumber,
            request.PageSize
        );

        var result = await _mediator.SendAsync(query);

        return Ok(result.ToGetBooksResponse());
    }

    /// <summary>Добавление деталей книги</summary>
    [HttpPost("{id}/details")]
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AddBookDetailsResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> AddBookDetails(
        Guid id,
        [FromForm] AddBookDetailsRequest request)
    {
        if (request.CoverImage == null || request.CoverImage.Length == 0)
        {
            return BadRequest("Файл обложки обязателен.");
        }

        await using var fileStream = request.CoverImage.OpenReadStream();

        var command = new AddBookDetailsCommand(
            id,
            request.Description,
            fileStream,
            request.CoverImage.FileName,
            request.CoverImage.ContentType
        );

        var coverImagePath = await _mediator.SendAsync(command);

        return Ok(new AddBookDetailsResponse(id, coverImagePath));
    }

}