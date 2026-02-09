namespace PracticalWork.Library.Contracts.v1.Library.Request;

/// <summary>
/// Запрос на выдачу книги читателю
/// </summary>
/// <param name="BookId">Идентификатор книги</param>
/// <param name="ReaderId">Идентификатор читателя</param>
public sealed record BorrowBookRequest(
    Guid BookId,
    Guid ReaderId
);