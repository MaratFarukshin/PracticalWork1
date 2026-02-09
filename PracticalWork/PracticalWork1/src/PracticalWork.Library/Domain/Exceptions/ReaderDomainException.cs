namespace PracticalWork.Library.Domain.Exceptions;

/// <summary>
/// Доменное исключение для операций с читателями
/// </summary>
public sealed class ReaderDomainException : DomainException
{
    public ReaderDomainException(string message) : base(message)
    {
    }

    public ReaderDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

