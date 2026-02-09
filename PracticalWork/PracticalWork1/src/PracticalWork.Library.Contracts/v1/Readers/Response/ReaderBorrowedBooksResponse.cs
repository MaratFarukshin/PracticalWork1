namespace PracticalWork.Library.Contracts.v1.Readers.Response;

/// <summary>
/// Информация о взятой книге читателя
/// </summary>
/// <param name="BookId">Идентификатор книги</param>
/// <param name="Title">Название книги</param>
/// <param name="Authors">Авторы</param>
/// <param name="BorrowDate">Дата выдачи книги</param>
/// <param name="DueDate">Срок возврата книги</param>
public sealed record ReaderBorrowedBookResponse(
    Guid BookId,
    string Title,
    IReadOnlyList<string> Authors,
    DateOnly BorrowDate,
    DateOnly DueDate
);

/// <summary>
/// Ответ со списком взятых книг читателя
/// </summary>
/// <param name="ReaderId">Идентификатор читателя</param>
/// <param name="Books">Список взятых книг</param>
public sealed record GetReaderBorrowedBooksResponse(
    Guid ReaderId,
    IReadOnlyList<ReaderBorrowedBookResponse> Books
);