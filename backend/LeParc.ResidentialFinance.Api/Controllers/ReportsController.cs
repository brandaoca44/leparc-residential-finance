using LeParc.ResidentialFinance.Application.DTOs.Reports;
using LeParc.ResidentialFinance.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeParc.ResidentialFinance.Api.Controllers;

/// <summary>
/// Disponibiliza relatórios financeiros consolidados.
/// </summary>
[ApiController]
[Route("api/reports")]
[Produces("application/json")]
public sealed class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Retorna o resumo financeiro geral.
    /// </summary>
    /// <response code="200">
    /// Resumo financeiro calculado com sucesso.
    /// </response>
    [HttpGet("summary")]
    [ProducesResponseType(
        typeof(FinancialSummaryResponse),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<FinancialSummaryResponse>> GetSummary(
        CancellationToken cancellationToken)
    {
        var summary = await _reportService.GetFinancialSummaryAsync(
            cancellationToken);

        return Ok(summary);
    }
}