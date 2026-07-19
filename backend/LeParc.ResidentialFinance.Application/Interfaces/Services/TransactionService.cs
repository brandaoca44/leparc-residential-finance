using LeParc.ResidentialFinance.Application.DTOs.Transactions;
using LeParc.ResidentialFinance.Application.Exceptions;
using LeParc.ResidentialFinance.Application.Interfaces.Repositories;
using LeParc.ResidentialFinance.Application.Interfaces.Services;
using LeParc.ResidentialFinance.Domain.Entities;
using LeParc.ResidentialFinance.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace LeParc.ResidentialFinance.Application.Services;

/// <summary>
/// Implementa as regras de negócio relacionadas às transações financeiras.
/// </summary>
public sealed class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IPersonRepository _personRepository;
    private readonly ILogger<TransactionService> _logger;

    /// <summary>
    /// Inicializa uma nova instância do serviço de transações.
    /// </summary>
    /// <param name="transactionRepository">
    /// Repositório responsável pela persistência das transações.
    /// </param>
    /// <param name="personRepository">
    /// Repositório utilizado para consultar as pessoas cadastradas.
    /// </param>
    /// <param name="logger">
    /// Componente responsável pelo registro dos eventos relevantes do serviço.
    /// </param>
    public TransactionService(
        ITransactionRepository transactionRepository,
        IPersonRepository personRepository,
        ILogger<TransactionService> logger)
    {
        _transactionRepository = transactionRepository;
        _personRepository = personRepository;
        _logger = logger;
    }

    /// <summary>
    /// Cria uma nova transação financeira.
    /// </summary>
    /// <param name="request">Dados necessários para a criação.</param>
    /// <param name="cancellationToken">
    /// Token utilizado para cancelar a operação assíncrona.
    /// </param>
    /// <returns>Os dados da transação criada.</returns>
    public async Task<TransactionResponse> CreateAsync(
        CreateTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateRequest(request);

        var person = await _personRepository.GetByIdAsync(
            request.PersonId,
            cancellationToken);

        if (person is null)
        {
            _logger.LogWarning(
                "Transaction creation rejected because person {PersonId} was not found.",
                request.PersonId);

            throw new NotFoundException(
                "A pessoa informada não foi encontrada.");
        }

        /*
         * A regra é validada no serviço para devolver uma mensagem clara
         * antes da persistência. Ela também deve permanecer protegida
         * dentro da entidade de domínio.
         */
        if (person.Age < 18 && request.Type == TransactionType.Income)
        {
            _logger.LogWarning(
                "Income creation rejected for minor person {PersonId}.",
                person.Id);

            throw new InvalidOperationException(
                "Pessoas menores de 18 anos só podem registrar despesas.");
        }

        var description = NormalizeDescription(request.Description);

        var transaction = new FinancialTransaction(
            description,
            request.Amount,
            request.Type,
            person);

        await _transactionRepository.AddAsync(
            transaction,
            cancellationToken);

        await _transactionRepository.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Transaction {TransactionId} created successfully for person {PersonId}.",
            transaction.Id,
            person.Id);

        return MapToResponse(transaction);
    }

    /// <summary>
    /// Retorna todas as transações financeiras cadastradas.
    /// </summary>
    /// <param name="cancellationToken">
    /// Token utilizado para cancelar a operação assíncrona.
    /// </param>
    /// <returns>Lista contendo as transações cadastradas.</returns>
    public async Task<IReadOnlyList<TransactionResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var transactions =
            await _transactionRepository.GetAllAsync(cancellationToken);

        return transactions
            .Select(MapToResponse)
            .ToList();
    }

    /// <summary>
    /// Valida os valores básicos necessários para criar uma transação.
    /// </summary>
    private static void ValidateRequest(
        CreateTransactionRequest request)
    {
        if (request.PersonId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.PersonId),
                "O identificador da pessoa deve ser maior que zero.");
        }

        if (request.Amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.Amount),
                "O valor da transação deve ser maior que zero.");
        }

        if (!Enum.IsDefined(request.Type))
        {
            throw new ArgumentException(
                "O tipo da transação é inválido.",
                nameof(request.Type));
        }
    }

    /// <summary>
    /// Remove espaços desnecessários da descrição informada.
    /// </summary>
    private static string NormalizeDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException(
                "A descrição da transação é obrigatória.",
                nameof(description));
        }

        return string.Join(
            ' ',
            description.Trim().Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries));
    }

    /// <summary>
    /// Converte uma entidade financeira em um objeto de resposta.
    /// </summary>
    private static TransactionResponse MapToResponse(
        FinancialTransaction transaction)
    {
        return new TransactionResponse(
            transaction.Id,
            transaction.Description,
            transaction.Amount,
            transaction.Type,
            transaction.PersonId,
            transaction.Person.Name);
    }
}