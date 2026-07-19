namespace LeParc.ResidentialFinance.Application.Exceptions;

/// <summary>
/// Indica que um recurso solicitado não foi encontrado.
/// </summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message)
    {
    }
}