namespace LeParc.ResidentialFinance.Application.Interfaces.Repositories;

/// <summary>
/// Define operações de consulta para relatórios financeiros.
/// </summary>
public interface IReportRepository
{
    /// <summary>
    /// Obtém os indicadores gerais utilizados pelo relatório financeiro.
    /// </summary>
    Task<(
        int TotalPeople,
        int TotalTransactions,
        decimal TotalIncome,
        decimal TotalExpense)> GetFinancialTotalsAsync(
            CancellationToken cancellationToken = default);
}