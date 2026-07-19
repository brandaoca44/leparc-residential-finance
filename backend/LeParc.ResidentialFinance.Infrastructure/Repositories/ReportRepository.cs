using LeParc.ResidentialFinance.Application.Interfaces.Repositories;
using LeParc.ResidentialFinance.Domain.Enums;
using LeParc.ResidentialFinance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeParc.ResidentialFinance.Infrastructure.Repositories;

/// <summary>
/// Implementa consultas agregadas para relatórios financeiros.
/// </summary>
public sealed class ReportRepository : IReportRepository
{
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// Inicializa uma nova instância do repositório de relatórios.
    /// </summary>
    public ReportRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Obtém todos os indicadores utilizados pelo resumo financeiro.
    /// </summary>
    public async Task<(
        int TotalPeople,
        int TotalTransactions,
        decimal TotalIncome,
        decimal TotalExpense)> GetFinancialTotalsAsync(
        CancellationToken cancellationToken = default)
    {
        var totalPeople = await _dbContext.People
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var totalTransactions = await _dbContext.FinancialTransactions
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var totalIncome = await _dbContext.FinancialTransactions
            .AsNoTracking()
            .Where(transaction =>
                transaction.Type == TransactionType.Income)
            .SumAsync(
                transaction => (double)transaction.Amount,
                cancellationToken);

        var totalExpense = await _dbContext.FinancialTransactions
            .AsNoTracking()
            .Where(transaction =>
                transaction.Type == TransactionType.Expense)
            .SumAsync(
                transaction => (double)transaction.Amount,
                cancellationToken);

        return
        (
            TotalPeople: totalPeople,
            TotalTransactions: totalTransactions,
            TotalIncome: Convert.ToDecimal(totalIncome),
            TotalExpense: Convert.ToDecimal(totalExpense)
        );
    }
}