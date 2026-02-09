namespace PracticalWork.Library.Contracts.v1.Readers.Response;

/// <summary>
/// Ответ на продление срока действия карточки читателя
/// </summary>
/// <param name="Id">Идентификатор читателя</param>
/// <param name="ExpiryDate">Новая дата окончания действия карточки</param>
public sealed record ExtendReaderResponse(Guid Id, DateOnly ExpiryDate);


