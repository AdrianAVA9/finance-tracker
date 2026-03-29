using FluentValidation;
using System.Linq;

namespace Fintrack.Server.Application.Expenses.Commands
{
    public class UpdateExpenseCommandValidator : AbstractValidator<UpdateExpenseCommand>
    {
        public UpdateExpenseCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.TotalAmount).GreaterThan(0);
            RuleFor(x => x.Date).NotEmpty();
            RuleFor(x => x.Items).NotEmpty().WithMessage("Expense must contain at least one item.");

            RuleFor(x => x).Must(command => 
            {
                if (command.Items == null || !command.Items.Any()) return false;
                decimal itemSum = command.Items.Sum(i => i.ItemAmount);
                return itemSum == command.TotalAmount;
            })
            .WithName("MathematicalIntegrity")
            .WithMessage(x => $"Strict Math Validation Failed: The sum of items does not equal the Total Amount ({x.TotalAmount}).");

            RuleFor(x => x.Frequency)
                .NotNull()
                .When(x => x.IsRecurring)
                .WithMessage("Frequency is required when generating a recurring expense template.");
        }
    }
}
