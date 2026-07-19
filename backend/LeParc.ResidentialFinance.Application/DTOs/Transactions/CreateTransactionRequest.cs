using System.ComponentModel.DataAnnotations;
using LeParc.ResidentialFinance.Domain.Enums;

namespace LeParc.ResidentialFinance.Application.DTOs.Transactions;

/// <summary>
/// Dados necessários para cadastrar uma transação financeira.
/// </summary>
public sealed class CreateTransactionRequest : IValidatableObject
{
    /// <summary>
    /// Descrição da transação.
    /// </summary>
    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(
        200,
        MinimumLength = 2,
        ErrorMessage = "A descrição deve possuir entre 2 e 200 caracteres.")]
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Valor monetário da transação.
    /// </summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// Tipo da transação: Expense ou Income.
    /// </summary>
    [EnumDataType(
        typeof(TransactionType),
        ErrorMessage = "O tipo da transação é inválido.")]
    public TransactionType Type { get; init; }

    /// <summary>
    /// Identificador da pessoa responsável pela transação.
    /// </summary>
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "O identificador da pessoa deve ser maior que zero.")]
    public int PersonId { get; init; }

    /// <summary>
    /// Executa validações que não devem depender da cultura do sistema operacional.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (Amount <= 0)
        {
            yield return new ValidationResult(
                "O valor da transação deve ser maior que zero.",
                [nameof(Amount)]);
        }
    }
}