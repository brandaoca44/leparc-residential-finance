using LeParc.ResidentialFinance.Application.DTOs.People;
using LeParc.ResidentialFinance.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeParc.ResidentialFinance.Api.Controllers;

/// <summary>
/// Gerencia o cadastro de pessoas.
/// </summary>
[ApiController]
[Route("api/people")]
[Produces("application/json")]
public sealed class PeopleController : ControllerBase
{
    private readonly IPersonService _personService;

    public PeopleController(IPersonService personService)
    {
        _personService = personService;
    }

    /// <summary>
    /// Cadastra uma nova pessoa.
    /// </summary>
    /// <param name="request">Nome e idade da pessoa.</param>
    /// <param name="cancellationToken">
    /// Token utilizado para cancelar a operação.
    /// </param>
    /// <returns>A pessoa cadastrada.</returns>
    [HttpPost]
    [ProducesResponseType(
        typeof(PersonResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PersonResponse>> CreateAsync(
        [FromBody] CreatePersonRequest request,
        CancellationToken cancellationToken)
    {
        var person = await _personService.CreateAsync(
            request,
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            person);
    }

    /// <summary>
    /// Lista todas as pessoas cadastradas.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<PersonResponse>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PersonResponse>>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var people = await _personService.GetAllAsync(
            cancellationToken);

        return Ok(people);
    }

    /// <summary>
    /// Exclui uma pessoa e todas as suas transações.
    /// </summary>
    /// <param name="id">Identificador da pessoa.</param>
    [HttpDelete("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        await _personService.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}