namespace PracticalWork.Library.Domain.Exceptions;

/// <summary>
/// Доменное исключение для операций с книгами
/// </summary>
public sealed class BookDomainException : DomainException
{
    public BookDomainException(string message) : base(message)
    {
    }

    public BookDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

