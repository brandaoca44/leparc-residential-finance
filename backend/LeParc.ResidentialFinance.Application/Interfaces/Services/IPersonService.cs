using LeParc.ResidentialFinance.Application.DTOs.People;

namespace LeParc.ResidentialFinance.Application.Interfaces.Services;

/// <summary>
/// Define os casos de uso relacionados ao cadastro de pessoas.
/// </summary>
public interface IPersonService
{
    Task<PersonResponse> CreateAsync(
        CreatePersonRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PersonResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}