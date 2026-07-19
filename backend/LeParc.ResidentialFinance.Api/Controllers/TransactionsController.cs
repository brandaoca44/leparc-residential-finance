using LeParc.ResidentialFinance.Application.DTOs.Transactions;
using LeParc.ResidentialFinance.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeParc.ResidentialFinance.Api.Controllers;

/// <summary>
/// Gerencia o cadastro e a consulta de transações financeiras.
/// </summary>
[ApiController]
[Route("api/transactions")]
[Produces("application/json")]
public sealed class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(
        ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    /// <summary>
    /// Cadastra uma nova transação financeira.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(
        typeof(TransactionResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionResponse>> CreateAsync(
        [FromBody] CreateTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var transaction = await _transactionService.CreateAsync(
            request,
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            transaction);
    }

    /// <summary>
    /// Lista todas as transações financeiras cadastradas.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<TransactionResponse>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TransactionResponse>>>
        GetAllAsync(CancellationToken cancellationToken)
    {
        var transactions =
            await _transactionService.GetAllAsync(cancellationToken);

        return Ok(transactions);
    }
}