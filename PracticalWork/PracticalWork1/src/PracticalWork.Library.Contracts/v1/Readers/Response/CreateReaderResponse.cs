namespace PracticalWork.Library.Contracts.v1.Readers.Response;

/// <summary>
/// Ответ на создание карточки читателя
/// </summary>
/// <param name="Id">Идентификатор читателя</param>
public sealed record CreateReaderResponse(Guid Id);

