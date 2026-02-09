namespace PracticalWork.Library.Contracts.v1.Library.Response;

/// <summary>
/// Ответ на возврат книги
/// </summary>
/// <param name="BookId">Идентификатор книги</param>
/// <param name="ReaderId">Идентификатор читателя</param>
/// <param name="ReturnDate">Дата возврата книги</param>
/// <param name="WasOverdue">Была ли книга возвращена с просрочкой</param>
public sealed record ReturnBookResponse(
    Guid BookId,
    Guid ReaderId,
    DateOnly ReturnDate,
    bool WasOverdue
);

