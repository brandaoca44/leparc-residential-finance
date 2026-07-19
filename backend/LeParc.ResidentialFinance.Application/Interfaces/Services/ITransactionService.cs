using LeParc.ResidentialFinance.Application.DTOs.Transactions;

namespace LeParc.ResidentialFinance.Application.Interfaces.Services;

/// <summary>
/// Define os casos de uso relacionados às transações financeiras.
/// </summary>
public interface ITransactionService
{
    Task<TransactionResponse> CreateAsync(
        CreateTransactionRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TransactionResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);
}