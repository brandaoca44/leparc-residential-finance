using LeParc.ResidentialFinance.Domain.Enums;

namespace LeParc.ResidentialFinance.Application.DTOs.Transactions;

/// <summary>
/// Representa os dados públicos de uma transação financeira.
/// </summary>
public sealed record TransactionResponse(
    int Id,
    string Description,
    decimal Amount,
    TransactionType Type,
    int PersonId,
    string PersonName);