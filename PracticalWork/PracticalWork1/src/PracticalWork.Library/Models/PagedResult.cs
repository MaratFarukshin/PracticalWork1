namespace PracticalWork.Library.Models;

/// <summary>
/// Результат пагинации
/// </summary>
/// <typeparam name="T">Тип элементов</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>Список элементов</summary>
    public IReadOnlyList<T> Items { get; set; }

    /// <summary>Общее количество элементов</summary>
    public int TotalCount { get; set; }

    /// <summary>Номер текущей страницы</summary>
    public int PageNumber { get; set; }

    /// <summary>Размер страницы</summary>
    public int PageSize { get; set; }

    /// <summary>Общее количество страниц</summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

