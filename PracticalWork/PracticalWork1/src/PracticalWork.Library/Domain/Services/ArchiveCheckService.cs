using PracticalWork.Library.Models;

namespace PracticalWork.Library.Domain.Services;

/// <summary>
/// Доменный сервис для проверки возможности архивирования
/// </summary>
public sealed class ArchiveCheckService
{
    private readonly BookAvailabilityService _bookAvailabilityService;

    public ArchiveCheckService(BookAvailabilityService bookAvailabilityService)
    {
        _bookAvailabilityService = bookAvailabilityService;
    }

    /// <summary>
    /// Проверить, может ли книга быть заархивирована
    /// </summary>
    public bool CanBeArchived(Book book)
    {
        return _bookAvailabilityService.CanBeArchived(book);
    }
}

