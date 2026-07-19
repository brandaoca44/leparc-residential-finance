namespace LeParc.ResidentialFinance.Application.DTOs.People;

/// <summary>
/// Representa os dados públicos de uma pessoa.
/// </summary>
public sealed record PersonResponse(
    int Id,
    string Name,
    int Age);