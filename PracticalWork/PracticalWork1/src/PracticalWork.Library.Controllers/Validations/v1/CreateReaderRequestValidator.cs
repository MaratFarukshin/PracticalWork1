using FluentValidation;
using PracticalWork.Library.Contracts.v1.Readers.Request;
using System.Text.RegularExpressions;

namespace PracticalWork.Library.Controllers.Validations.v1;

public sealed class CreateReaderRequestValidator : AbstractValidator<CreateReaderRequest>
{
    private static readonly Regex PhoneNumberRegex = new(@"^\+?\d{10,12}$", RegexOptions.Compiled);

    public CreateReaderRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("ФИО читателя обязательно.")
            .MaximumLength(200).WithMessage("ФИО не может превышать 200 символов.")
            .Must(name => name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length >= 2)
            .WithMessage("ФИО должно содержать минимум два слова (имя и фамилию).");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Номер телефона обязателен.")
            .MaximumLength(12).WithMessage("Номер телефона не может превышать 12 символов.")
            .Must(phone => PhoneNumberRegex.IsMatch(phone))
            .WithMessage("Номер телефона должен быть в формате +XXXXXXXXXXX или XXXXXXXXXX (10-12 цифр).");

        RuleFor(x => x.ExpiryDate)
            .NotEmpty().WithMessage("Дата окончания действия карточки обязательна.")
            .GreaterThan(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Дата окончания действия карточки должна быть в будущем.");
    }
}

