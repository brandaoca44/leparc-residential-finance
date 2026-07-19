using System.ComponentModel.DataAnnotations;

namespace LeParc.ResidentialFinance.Application.DTOs.People;

/// <summary>
/// Dados necessários para cadastrar uma pessoa.
/// </summary>
public sealed class CreatePersonRequest
{
    /// <summary>
    /// Nome completo da pessoa.
    /// </summary>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(
        150,
        MinimumLength = 2,
        ErrorMessage = "O nome deve possuir entre 2 e 150 caracteres.")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Idade da pessoa.
    /// </summary>
    [Range(
        0,
        130,
        ErrorMessage = "A idade deve estar entre 0 e 130 anos.")]
    public int Age { get; init; }
}