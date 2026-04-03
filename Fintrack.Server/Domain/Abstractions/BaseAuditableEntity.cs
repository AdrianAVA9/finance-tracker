using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Exceptions;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Domain.Invoices;
using Fintrack.Server.Domain.SavingsGoals;
using Fintrack.Server.Domain.Users;

using System;

namespace Fintrack.Server.Domain.Abstractions
{
    public abstract class BaseAuditableEntity : IAuditableEntity
    {
        public int Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public string? CreatedBy { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
