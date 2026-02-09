namespace PracticalWork.Library.Domain.Exceptions;

/// <summary>
/// Доменное исключение для операций библиотеки
/// </summary>
public sealed class LibraryDomainException : DomainException
{
    public LibraryDomainException(string message) : base(message)
    {
    }

    public LibraryDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

