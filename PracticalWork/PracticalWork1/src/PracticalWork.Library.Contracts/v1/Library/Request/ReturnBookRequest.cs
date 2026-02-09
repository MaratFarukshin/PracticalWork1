namespace PracticalWork.Library.Contracts.v1.Library.Request;

/// <summary>
/// Запрос на возврат книги
/// </summary>
/// <param name="BookId">Идентификатор книги</param>
/// <param name="ReaderId">Идентификатор читателя</param>
public sealed record ReturnBookRequest(
    Guid BookId,
    Guid ReaderId
);

