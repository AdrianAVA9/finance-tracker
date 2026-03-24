using Fintrack.Server.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Fintrack.Server.Domain.Abstractions;

namespace Fintrack.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ExpenseCategoryGroup> ExpenseCategoryGroups { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<IncomeCategory> IncomeCategories { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<RecurringIncome> RecurringIncomes { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseItem> ExpenseItems { get; set; }
        public DbSet<RecurringExpense> RecurringExpenses { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<SavingsGoal> SavingsGoals { get; set; }

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries<IAuditableEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Global ValueConverter for DateTime to ensure UTC for PostgreSQL
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                v => v);

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)) : v,
                v => v);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(nullableDateTimeConverter);
                    }
                }
            }

            // Configure Delete Behaviors to avoid infinite cascade paths
            builder.Entity<ExpenseCategory>()
                .HasOne(ec => ec.Group)
                .WithMany(g => g.Categories)
                .HasForeignKey(ec => ec.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ExpenseItem>()
                .HasOne(ei => ei.Expense)
                .WithMany(e => e.Items)
                .HasForeignKey(ei => ei.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ExpenseItem>()
                .HasOne(ei => ei.Category)
                .WithMany()
                .HasForeignKey(ei => ei.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RecurringExpense>()
                .HasOne(re => re.Category)
                .WithMany()
                .HasForeignKey(re => re.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Expense>()
                .HasOne(e => e.Invoice)
                .WithMany()
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<InvoiceItem>()
                .HasOne(i => i.AssignedCategory)
                .WithMany()
                .HasForeignKey(i => i.AssignedCategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Budget>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Budgets)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Budget>()
                .HasIndex(b => new { b.UserId, b.CategoryId, b.Month, b.Year })
                .IsUnique();

            builder.Entity<Income>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Incomes)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RecurringIncome>()
                .HasOne(ri => ri.Category)
                .WithMany(c => c.RecurringIncomes)
                .HasForeignKey(ri => ri.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
