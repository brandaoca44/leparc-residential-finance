namespace LeParc.ResidentialFinance.Application.DTOs.Reports;

/// <summary>
/// Representa o resumo geral do sistema financeiro.
/// </summary>
public sealed record FinancialSummaryResponse(
    int TotalPeople,
    int TotalTransactions,
    decimal TotalIncome,
    decimal TotalExpense,
    decimal Balance);