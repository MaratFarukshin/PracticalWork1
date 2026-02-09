using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Application.Abstractions;
using PracticalWork.Library.Application.Queries.Books;

namespace PracticalWork.Library.Application.Handlers.Queries;

/// <summary>
/// Обработчик запроса получения деталей книги
/// </summary>
public sealed class GetBookDetailsQueryHandler : IQueryHandler<GetBookDetailsQuery, Models.Book>
{
    private readonly IBookRepository _bookRepository;

    public GetBookDetailsQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Models.Book> HandleAsync(GetBookDetailsQuery query, CancellationToken cancellationToken = default)
    {
        return await _bookRepository.GetBookByIdAsync(query.Id);
    }
}

