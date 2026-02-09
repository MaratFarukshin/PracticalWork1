using FluentValidation;
using PracticalWork.Library.Contracts.v1.Library.Request;

namespace PracticalWork.Library.Controllers.Validations.v1;

/// <summary>
/// Валидация запроса на выдачу книги
/// </summary>
public sealed class BorrowBookRequestValidator : AbstractValidator<BorrowBookRequest>
{
    public BorrowBookRequestValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("Идентификатор книги обязателен.");

        RuleFor(x => x.ReaderId)
            .NotEmpty().WithMessage("Идентификатор читателя обязателен.");
    }
}