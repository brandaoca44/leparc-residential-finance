using LeParc.ResidentialFinance.Domain.Entities;

namespace LeParc.ResidentialFinance.Application.Interfaces.Repositories;

/// <summary>
/// Abstrai as operações de persistência relacionadas às transações.
/// </summary>
public interface ITransactionRepository
{
    Task<IReadOnlyList<FinancialTransaction>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        FinancialTransaction transaction,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}