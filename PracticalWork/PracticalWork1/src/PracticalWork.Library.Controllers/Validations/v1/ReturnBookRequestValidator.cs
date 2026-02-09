using FluentValidation;
using PracticalWork.Library.Contracts.v1.Library.Request;

namespace PracticalWork.Library.Controllers.Validations.v1;

/// <summary>
/// Валидация запроса на возврат книги
/// </summary>
public sealed class ReturnBookRequestValidator : AbstractValidator<ReturnBookRequest>
{
    public ReturnBookRequestValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("Идентификатор книги обязателен.");

        RuleFor(x => x.ReaderId)
            .NotEmpty().WithMessage("Идентификатор читателя обязателен.");
    }
}

