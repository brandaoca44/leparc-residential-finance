using LeParc.ResidentialFinance.Application.DTOs.Reports;
using LeParc.ResidentialFinance.Application.Interfaces.Repositories;
using LeParc.ResidentialFinance.Application.Interfaces.Services;

namespace LeParc.ResidentialFinance.Application.Services;

/// <summary>
/// Implementa os casos de uso relacionados aos relatórios financeiros.
/// </summary>
public sealed class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;

    /// <summary>
    /// Inicializa uma nova instância do serviço de relatórios.
    /// </summary>
    public ReportService(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    /// <summary>
    /// Obtém o resumo geral do sistema financeiro.
    /// </summary>
    public async Task<FinancialSummaryResponse> GetFinancialSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var totals = await _reportRepository.GetFinancialTotalsAsync(
            cancellationToken);

        var balance = totals.TotalIncome - totals.TotalExpense;

        return new FinancialSummaryResponse(
            TotalPeople: totals.TotalPeople,
            TotalTransactions: totals.TotalTransactions,
            TotalIncome: totals.TotalIncome,
            TotalExpense: totals.TotalExpense,
            Balance: balance);
    }
}