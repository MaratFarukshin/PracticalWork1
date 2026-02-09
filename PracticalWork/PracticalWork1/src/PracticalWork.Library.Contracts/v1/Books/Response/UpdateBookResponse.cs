namespace PracticalWork.Library.Contracts.v1.Books.Response;

/// <summary>
/// Ответ на обновление книги
/// </summary>
/// <param name="Id">Идентификатор книги</param>
public sealed record UpdateBookResponse(Guid Id);

