using MediatR;
using Fintrack.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Server.Application.Incomes.Commands
{
    public record DeleteIncomeCommand(int Id, string UserId) : IRequest<bool>;

    public class DeleteIncomeCommandHandler : IRequestHandler<DeleteIncomeCommand, bool>
    {
        private readonly ApplicationDbContext _dbContext;

        public DeleteIncomeCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteIncomeCommand request, CancellationToken cancellationToken)
        {
            var income = await _dbContext.Incomes
                .FirstOrDefaultAsync(i => i.Id == request.Id && i.UserId == request.UserId, cancellationToken);

            if (income == null) return false;

            // Optional: Deactivate associated recurring template? 
            // In a simple hard-delete scenario, we just remove the record. 
            // If there's a recurring template matching the source/amount, it might continue generating.
            // But since the user asked for hard-delete specifically for records, we remove the record.
            
            _dbContext.Incomes.Remove(income);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
