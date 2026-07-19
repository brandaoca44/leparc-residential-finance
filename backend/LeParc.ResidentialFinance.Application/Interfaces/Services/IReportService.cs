using LeParc.ResidentialFinance.Application.DTOs.Reports;

namespace LeParc.ResidentialFinance.Application.Interfaces.Services;

/// <summary>
/// Define os casos de uso relacionados aos relatórios financeiros.
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Retorna o resumo financeiro geral do sistema.
    /// </summary>
    Task<FinancialSummaryResponse> GetFinancialSummaryAsync(
        CancellationToken cancellationToken = default);
}