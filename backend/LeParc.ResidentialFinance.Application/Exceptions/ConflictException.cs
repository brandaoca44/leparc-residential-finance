namespace LeParc.ResidentialFinance.Application.Exceptions;

/// <summary>
/// Indica que uma operação viola uma regra de integridade ou negócio.
/// </summary>
public sealed class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }
}