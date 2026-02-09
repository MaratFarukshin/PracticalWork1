namespace PracticalWork.Reports.Models;

/// <summary>
/// Результат с пагинацией
/// </summary>
public sealed class PagedResult<T>
{
    public required IReadOnlyList<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

