namespace LeParc.ResidentialFinance.Domain.Entities;

/// <summary>
/// Representa uma pessoa responsável por receitas e despesas residenciais.
/// </summary>
public class Person
{
    /// <summary>
    /// Identificador único gerado automaticamente pelo banco de dados.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Nome completo da pessoa.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Idade atual da pessoa.
    /// </summary>
    public int Age { get; private set; }

    /// <summary>
    /// Transações financeiras pertencentes à pessoa.
    /// </summary>
    public ICollection<FinancialTransaction> Transactions { get; private set; }
        = new List<FinancialTransaction>();

    protected Person()
    {
    }

    public Person(string name, int age)
    {
        UpdateName(name);
        UpdateAge(age);
    }

    private void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "O nome da pessoa é obrigatório.",
                nameof(name));
        }

        Name = name.Trim();
    }

    private void UpdateAge(int age)
    {
        if (age < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(age),
                "A idade não pode ser negativa.");
        }

        if (age > 130)
        {
            throw new ArgumentOutOfRangeException(
                nameof(age),
                "A idade informada é inválida.");
        }

        Age = age;
    }
}