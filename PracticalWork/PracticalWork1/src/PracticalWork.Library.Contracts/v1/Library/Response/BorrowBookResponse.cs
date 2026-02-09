namespace PracticalWork.Library.Contracts.v1.Library.Response;

/// <summary>
/// Ответ на выдачу книги читателю
/// </summary>
/// <param name="BookId">Идентификатор книги</param>
/// <param name="ReaderId">Идентификатор читателя</param>
/// <param name="BorrowDate">Дата выдачи книги</param>
/// <param name="DueDate">Срок возврата книги</param>
public sealed record BorrowBookResponse(
    Guid BookId,
    Guid ReaderId,
    DateOnly BorrowDate,
    DateOnly DueDate
);