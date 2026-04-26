namespace Fintrack.Server.Domain.IncomeCategories;

public interface IIncomeCategoryRepository
{
    Task<IncomeCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<IncomeCategory>> GetAllByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    /// <summary>Categories created by the user (excludes system-seeded rows with <c>UserId == null</c>).</summary>
    Task<IReadOnlyList<IncomeCategory>> GetUserOwnedByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    void Add(IncomeCategory incomeCategory);

    void Update(IncomeCategory incomeCategory);

    void Remove(IncomeCategory incomeCategory);
}
