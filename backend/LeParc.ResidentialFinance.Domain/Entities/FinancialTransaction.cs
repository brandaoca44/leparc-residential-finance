using LeParc.ResidentialFinance.Domain.Enums;

namespace LeParc.ResidentialFinance.Domain.Entities;

/// <summary>
/// Representa uma receita ou despesa vinculada a uma pessoa.
/// </summary>
public class FinancialTransaction
{
    /// <summary>
    /// Identificador único gerado automaticamente pelo banco de dados.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Descrição da movimentação financeira.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Valor positivo da movimentação.
    /// O tipo determina se o valor representa receita ou despesa.
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// Tipo da transação: despesa ou receita.
    /// </summary>
    public TransactionType Type { get; private set; }

    /// <summary>
    /// Identificador da pessoa responsável pela transação.
    /// </summary>
    public int PersonId { get; private set; }

    /// <summary>
    /// Pessoa vinculada à transação.
    /// </summary>
    public Person Person { get; private set; } = null!;

    protected FinancialTransaction()
    {
    }

    public FinancialTransaction(
        string description,
        decimal amount,
        TransactionType type,
        Person person)
    {
        if (person is null)
        {
            throw new ArgumentNullException(
                nameof(person),
                "A pessoa vinculada à transação é obrigatória.");
        }

        UpdateDescription(description);
        UpdateAmount(amount);
        ValidateTransactionType(type);

        /*
         * Regra principal criada pra esse desafio:
         * pessoas menores de 18 anos podem registrar apenas despesas.
         */
        if (person.Age < 18 && type == TransactionType.Income)
        {
            throw new InvalidOperationException(
                "Pessoas menores de 18 anos não podem cadastrar receitas.");
        }

        Type = type;
        Person = person;
        PersonId = person.Id;
    }

    private void UpdateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException(
                "A descrição da transação é obrigatória.",
                nameof(description));
        }

        Description = description.Trim();
    }

    private void UpdateAmount(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "O valor da transação deve ser maior que zero.");
        }

        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    private static void ValidateTransactionType(TransactionType type)
    {
        if (!Enum.IsDefined(type))
        {
            throw new ArgumentException(
                "O tipo da transação é inválido.",
                nameof(type));
        }
    }
}