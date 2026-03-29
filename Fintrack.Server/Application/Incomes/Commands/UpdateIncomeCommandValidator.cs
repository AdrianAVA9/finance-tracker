using FluentValidation;

namespace Fintrack.Server.Application.Incomes.Commands
{
    public class UpdateIncomeCommandValidator : AbstractValidator<UpdateIncomeCommand>
    {
        public UpdateIncomeCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Source).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Date).NotEmpty();
            RuleFor(x => x.CategoryId).GreaterThan(0);

            RuleFor(x => x.Frequency)
                .NotNull()
                .When(x => x.IsRecurring)
                .WithMessage("La frecuencia es requerida cuando el ingreso es recurrente.");
        }
    }
}
