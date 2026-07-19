using LeParc.ResidentialFinance.Application.Interfaces.Repositories;
using LeParc.ResidentialFinance.Domain.Entities;
using LeParc.ResidentialFinance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LeParc.ResidentialFinance.Infrastructure.Repositories;

/// <summary>
/// Implementação das operações de persistência de pessoas usando EF Core.
/// </summary>
public sealed class PersonRepository : IPersonRepository
{
    private readonly AppDbContext _dbContext;

    public PersonRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Person>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        /*
         * AsNoTracking evita rastreamento desnecessário porque esta
         * consulta serve apenas para leitura.
         */
        return await _dbContext.People
            .AsNoTracking()
            .OrderBy(person => person.Name)
            .ThenBy(person => person.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Person?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.People
            .FirstOrDefaultAsync(
                person => person.Id == id,
                cancellationToken);
    }    

    public async Task AddAsync(
        Person person,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(person);

        await _dbContext.People.AddAsync(
            person,
            cancellationToken);
    }

    public void Remove(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);

        _dbContext.People.Remove(person);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}