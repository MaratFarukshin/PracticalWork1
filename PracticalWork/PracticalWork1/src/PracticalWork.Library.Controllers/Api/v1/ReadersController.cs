using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Application.Commands.Readers;
using PracticalWork.Library.Application.Queries.Readers;
using PracticalWork.Library.Contracts.v1.Readers.Request;
using PracticalWork.Library.Contracts.v1.Readers.Response;
using PracticalWork.Library.Controllers.Mappers.v1;

namespace PracticalWork.Library.Controllers.Api.v1;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/readers")]
public class ReadersController : Controller
{
    private readonly IMediator _mediator;

    public ReadersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary> Создание карточки читателя</summary>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CreateReaderResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CreateReader(CreateReaderRequest request)
    {
        var command = new CreateReaderCommand(request.ToReader());
        var result = await _mediator.SendAsync(command);

        return Ok(new CreateReaderResponse(result));
    }

    /// <summary> Продление срока действия карточки читателя</summary>
    [HttpPost("{id}/extend")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ExtendReaderResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ExtendReader(Guid id, ExtendReaderRequest request)
    {
        var command = new ExtendReaderCommand(id, request.NewExpiryDate);
        var newExpiryDate = await _mediator.SendAsync(command);

        return Ok(new ExtendReaderResponse(id, newExpiryDate));
    }

    /// <summary> Закрытие карточки читателя</summary>
    [HttpPost("{id}/close")]
    [Produces("application/json")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CloseReader(Guid id)
    {
        var command = new CloseReaderCommand(id);
        await _mediator.SendAsync(command);

        return Ok();
    }

    /// <summary> Получение взятых книг читателя</summary>
    [HttpGet("{id}/books")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GetReaderBorrowedBooksResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetBorrowedBooks(Guid id)
    {
        var query = new GetReaderBooksQuery(id);
        var borrowedBooks = await _mediator.SendAsync(query);

        return Ok(borrowedBooks.ToGetReaderBorrowedBooksResponse(id));
    }
}

