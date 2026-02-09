namespace PracticalWork.Library.Contracts.v1.Readers.Request;

/// <summary>
/// Запрос на создание карточки читателя
/// </summary>
/// <param name="FullName">ФИО читателя</param>
/// <param name="PhoneNumber">Номер телефона</param>
/// <param name="ExpiryDate">Дата окончания действия карточки</param>
public sealed record CreateReaderRequest(
    string FullName,
    string PhoneNumber,
    DateOnly ExpiryDate
);

