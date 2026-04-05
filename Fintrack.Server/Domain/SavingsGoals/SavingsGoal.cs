using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Users;

namespace Fintrack.Server.Domain.SavingsGoals;

public class SavingsGoal : Entity
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime? TargetDate { get; set; }
    public bool IsCompleted { get; set; }

    public virtual ApplicationUser? User { get; set; }
}
