using PracticalWork.Library.Enums;

namespace PracticalWork.Library.Domain.Entities;

/// <summary>
/// Сущность выдачи книги
/// </summary>
public sealed class Borrow
{
    /// <summary>Идентификатор выдачи</summary>
    public Guid Id { get; set; }

    /// <summary>Идентификатор книги</summary>
    public Guid BookId { get; set; }

    /// <summary>Идентификатор карточки читателя</summary>
    public Guid ReaderId { get; set; }

    /// <summary>Дата выдачи книги</summary>
    public DateOnly BorrowDate { get; set; }

    /// <summary>Срок возврата книги</summary>
    public DateOnly DueDate { get; set; }

    /// <summary>Фактическая дата возврата книги</summary>
    public DateOnly? ReturnDate { get; set; }

    /// <summary>Статус выдачи</summary>
    public BookIssueStatus Status { get; set; }

    /// <summary>Дата создания</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Дата обновления</summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Проверить, просрочена ли выдача
    /// </summary>
    public bool IsOverdue(DateOnly currentDate)
    {
        return Status == BookIssueStatus.Issued && currentDate > DueDate;
    }

    /// <summary>
    /// Вернуть книгу
    /// </summary>
    public void Return(DateOnly returnDate)
    {
        if (Status != BookIssueStatus.Issued)
            throw new InvalidOperationException("Книга уже возвращена или не была выдана.");

        ReturnDate = returnDate;
        Status = returnDate > DueDate ? BookIssueStatus.Overdue : BookIssueStatus.Returned;
    }
}

