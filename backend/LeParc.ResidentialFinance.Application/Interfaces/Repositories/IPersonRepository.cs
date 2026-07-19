using LeParc.ResidentialFinance.Domain.Entities;

namespace LeParc.ResidentialFinance.Application.Interfaces.Repositories;

/// <summary>
/// Abstrai as operações de persistência relacionadas a pessoas.
/// </summary>
public interface IPersonRepository
{
    Task<IReadOnlyList<Person>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Person?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);    

    Task AddAsync(
        Person person,
        CancellationToken cancellationToken = default);

    void Remove(Person person);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}