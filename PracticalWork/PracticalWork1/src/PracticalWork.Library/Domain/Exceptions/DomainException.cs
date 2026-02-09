namespace PracticalWork.Library.Domain.Exceptions;

/// <summary>
/// Базовое доменное исключение
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

