using LeParc.ResidentialFinance.Application.DTOs.People;
using LeParc.ResidentialFinance.Application.Exceptions;
using LeParc.ResidentialFinance.Application.Interfaces.Repositories;
using LeParc.ResidentialFinance.Application.Interfaces.Services;
using LeParc.ResidentialFinance.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace LeParc.ResidentialFinance.Application.Services;

/// <summary>
/// Implementa as regras de negócio relacionadas a pessoas.
/// </summary>
public sealed class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepository;
    private readonly ILogger<PersonService> _logger;

    /// <summary>
    /// Inicializa uma nova instância do serviço de pessoas.
    /// </summary>
    /// <param name="personRepository">
    /// Repositório responsável pela persistência das pessoas.
    /// </param>
    /// <param name="logger">
    /// Componente responsável pelo registro dos eventos relevantes do serviço.
    /// </param>
    public PersonService(
        IPersonRepository personRepository,
        ILogger<PersonService> logger)
    {
        _personRepository = personRepository;
        _logger = logger;
    }

    /// <summary>
    /// Cria uma nova pessoa.
    /// </summary>
    /// <param name="request">Dados necessários para a criação.</param>
    /// <param name="cancellationToken">
    /// Token utilizado para cancelar a operação assíncrona.
    /// </param>
    /// <returns>Os dados da pessoa criada.</returns>
    public async Task<PersonResponse> CreateAsync(
        CreatePersonRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var normalizedName = NormalizeName(request.Name);

        var person = new Person(normalizedName, request.Age);

        await _personRepository.AddAsync(person, cancellationToken);
        await _personRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Person {PersonId} created successfully.",
            person.Id);

        return MapToResponse(person);
    }

    /// <summary>
    /// Retorna todas as pessoas cadastradas.
    /// </summary>
    /// <param name="cancellationToken">
    /// Token utilizado para cancelar a operação assíncrona.
    /// </param>
    /// <returns>Lista contendo as pessoas cadastradas.</returns>
    public async Task<IReadOnlyList<PersonResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var people = await _personRepository.GetAllAsync(cancellationToken);

        return people
            .Select(MapToResponse)
            .ToList();
    }

    /// <summary>
    /// Exclui uma pessoa pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da pessoa.</param>
    /// <param name="cancellationToken">
    /// Token utilizado para cancelar a operação assíncrona.
    /// </param>
    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(id),
                "O identificador deve ser maior que zero.");
        }

        var person = await _personRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (person is null)
        {
            throw new NotFoundException(
                "A pessoa informada não foi encontrada.");
        }

        /*
         * As transações relacionadas serão removidas automaticamente
         * pelo relacionamento configurado com DeleteBehavior.Cascade.
         */
        _personRepository.Remove(person);

        await _personRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Person {PersonId} deleted successfully.",
            person.Id);
    }

    /// <summary>
    /// Remove espaços desnecessários do nome informado.
    /// </summary>
    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "O nome da pessoa é obrigatório.",
                nameof(name));
        }

        return string.Join(
            ' ',
            name.Trim()
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries));
    }

    /// <summary>
    /// Converte uma entidade de pessoa em um objeto de resposta.
    /// </summary>
    private static PersonResponse MapToResponse(Person person)
    {
        return new PersonResponse(
            person.Id,
            person.Name,
            person.Age);
    }
}