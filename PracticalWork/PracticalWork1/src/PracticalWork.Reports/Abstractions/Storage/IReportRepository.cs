using PracticalWork.Reports.Models;

namespace PracticalWork.Reports.Abstractions.Storage;

/// <summary>
/// Репозиторий для работы с отчетами
/// </summary>
public interface IReportRepository
{
    /// <summary>
    /// Создать отчет
    /// </summary>
    Task<Guid> CreateAsync(Report report, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить отчет
    /// </summary>
    Task UpdateAsync(Report report, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить отчет по ID
    /// </summary>
    Task<Report> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить все отчеты
    /// </summary>
    Task<IReadOnlyList<Report>> GetAllAsync(CancellationToken cancellationToken = default);
}

