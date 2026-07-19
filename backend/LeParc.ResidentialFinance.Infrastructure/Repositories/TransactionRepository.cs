using LeParc.ResidentialFinance.Application.Interfaces.Repositories;
using LeParc.ResidentialFinance.Domain.Entities;
using LeParc.ResidentialFinance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeParc.ResidentialFinance.Infrastructure.Repositories;

/// <summary>
/// Implementa a persistência de transações utilizando Entity Framework Core.
/// </summary>
public sealed class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _dbContext;

    public TransactionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<FinancialTransaction>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.FinancialTransactions
            .AsNoTracking()
            .Include(transaction => transaction.Person)
            .OrderByDescending(transaction => transaction.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        FinancialTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        await _dbContext.FinancialTransactions.AddAsync(
            transaction,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}