using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.IncomeCategories;
using Fintrack.Server.Domain.Incomes;
using Fintrack.Server.Domain.Invoices;
using Fintrack.Server.Domain.SavingsGoals;
using Fintrack.Server.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Fintrack.Server.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IUnitOfWork
{
    public const string AppSchema = "app";
    public const string IdentitySchema = "identity";

    private readonly IPublisher _publisher;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IPublisher publisher)
        : base(options)
    {
        _publisher = publisher;
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
    public DbSet<PendingReceiptExtractionJob> PendingReceiptExtractionJobs { get; set; }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        
        var domainEvents = ChangeTracker
            .Entries()
            .Where(e => e.Entity is IHasDomainEvents)
            .SelectMany(entry => ((IHasDomainEvents)entry.Entity).GetDomainEvents())
            .ToList();

        foreach (var entry in ChangeTracker.Entries().Where(e => e.Entity is IHasDomainEvents))
        {
            ((IHasDomainEvents)entry.Entity).ClearDomainEvents();
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        return result;
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

        ConfigureIdentitySchema(builder);
        ConfigureAppSchema(builder);

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

        builder.Entity<Expense>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(e => e.ExpenseCategory)
                .WithMany()
                .HasForeignKey(e => e.ExpenseCategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Invoice)
                .WithMany()
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.Items)
                .WithOne(i => i.Expense)
                .HasForeignKey(i => i.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Navigation(e => e.Items)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        builder.Entity<ExpenseItem>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(ei => ei.Category)
                .WithMany()
                .HasForeignKey(ei => ei.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<RecurringExpense>()
            .HasOne(re => re.Category)
            .WithMany()
            .HasForeignKey(re => re.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Invoice>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasMany(i => i.Items)
                .WithOne(ii => ii.Invoice)
                .HasForeignKey(ii => ii.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Navigation(i => i.Items)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        builder.Entity<InvoiceItem>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(i => i.AssignedCategory)
                .WithMany()
                .HasForeignKey(i => i.AssignedCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Budget>(entity =>
        {
            entity.ToTable("Budgets", AppSchema);
            entity.HasOne(b => b.Category)
                .WithMany(c => c.Budgets)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(b => new { b.UserId, b.CategoryId, b.Month, b.Year })
                .IsUnique();
        });

        builder.Entity<IncomeCategory>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        builder.Entity<Income>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.HasOne(i => i.Category)
                .WithMany(c => c.Incomes)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<RecurringIncome>()
            .HasOne(ri => ri.Category)
            .WithMany(c => c.RecurringIncomes)
            .HasForeignKey(ri => ri.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PendingReceiptExtractionJob>(entity =>
        {
            entity.ToTable("PendingReceiptExtractionJobs", AppSchema);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Status).HasConversion<int>();
        });
    }

    /// <summary>
    /// ASP.NET Identity stores live in <c>identity</c> schema (separate from application domain tables).
    /// </summary>
    private static void ConfigureIdentitySchema(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>().ToTable("AspNetUsers", IdentitySchema);
        builder.Entity<IdentityRole>().ToTable("AspNetRoles", IdentitySchema);
        builder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", IdentitySchema);
        builder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", IdentitySchema);
        builder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", IdentitySchema);
        builder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", IdentitySchema);
        builder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", IdentitySchema);
    }

    /// <summary>
    /// All domain aggregates use the <c>app</c> schema (Budgets already lived here; others move from default/public).
    /// </summary>
    private static void ConfigureAppSchema(ModelBuilder builder)
    {
        builder.Entity<ExpenseCategoryGroup>().ToTable("ExpenseCategoryGroups", AppSchema);
        builder.Entity<ExpenseCategory>().ToTable("ExpenseCategories", AppSchema);
        builder.Entity<IncomeCategory>().ToTable("IncomeCategories", AppSchema);
        builder.Entity<Income>().ToTable("Incomes", AppSchema);
        builder.Entity<RecurringIncome>().ToTable("RecurringIncomes", AppSchema);
        builder.Entity<Expense>().ToTable("Expenses", AppSchema);
        builder.Entity<ExpenseItem>().ToTable("ExpenseItems", AppSchema);
        builder.Entity<RecurringExpense>().ToTable("RecurringExpenses", AppSchema);
        builder.Entity<Invoice>().ToTable("Invoices", AppSchema);
        builder.Entity<InvoiceItem>().ToTable("InvoiceItems", AppSchema);
        builder.Entity<SavingsGoal>().ToTable("SavingsGoals", AppSchema);
    }
}
