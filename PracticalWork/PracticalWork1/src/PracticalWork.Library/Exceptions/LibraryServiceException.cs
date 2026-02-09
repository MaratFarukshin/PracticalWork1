namespace PracticalWork.Library.Exceptions;

/// <summary>
/// Исключение доменного уровня для операций библиотеки (выдача/возврат)
/// </summary>
public sealed class LibraryServiceException : AppException
{
    public LibraryServiceException(string message) : base(message)
    {
    }

    public LibraryServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}