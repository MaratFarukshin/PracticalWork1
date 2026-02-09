using FluentValidation;
using PracticalWork.Library.Contracts.v1.Readers.Request;

namespace PracticalWork.Library.Controllers.Validations.v1;

public sealed class ExtendReaderRequestValidator : AbstractValidator<ExtendReaderRequest>
{
    public ExtendReaderRequestValidator()
    {
        RuleFor(x => x.NewExpiryDate)
            .NotEmpty().WithMessage("Новая дата окончания действия карточки обязательна.")
            .GreaterThan(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Новая дата окончания действия карточки должна быть в будущем.");
    }
}


