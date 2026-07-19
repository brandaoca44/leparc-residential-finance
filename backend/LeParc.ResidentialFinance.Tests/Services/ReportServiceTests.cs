using FluentAssertions;
using LeParc.ResidentialFinance.Application.Interfaces.Repositories;
using LeParc.ResidentialFinance.Application.Services;
using Moq;

namespace LeParc.ResidentialFinance.Tests.Services;

/// <summary>
/// Contém os testes unitários responsáveis por validar
/// o cálculo e a montagem do resumo financeiro do sistema.
/// </summary>
public sealed class ReportServiceTests
{
    private readonly Mock<IReportRepository> _reportRepositoryMock;
    private readonly ReportService _reportService;

    /// <summary>
    /// Inicializa as dependências utilizadas em cada cenário de teste.
    /// </summary>
    public ReportServiceTests()
    {
        _reportRepositoryMock = new Mock<IReportRepository>();

        _reportService = new ReportService(
            _reportRepositoryMock.Object);
    }

    /// <summary>
    /// Verifica se o serviço retorna corretamente todos os indicadores
    /// fornecidos pelo repositório.
    /// </summary>
    [Fact]
    public async Task GetFinancialSummaryAsync_ShouldReturnCompleteSummary()
    {
        // Arrange
        const int totalPeople = 4;
        const int totalTransactions = 8;
        const decimal totalIncome = 5000.00m;
        const decimal totalExpense = 1750.00m;

        _reportRepositoryMock
            .Setup(repository => repository.GetFinancialTotalsAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((
                TotalPeople: totalPeople,
                TotalTransactions: totalTransactions,
                TotalIncome: totalIncome,
                TotalExpense: totalExpense));

        // Act
        var result = await _reportService.GetFinancialSummaryAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalPeople.Should().Be(totalPeople);
        result.TotalTransactions.Should().Be(totalTransactions);
        result.TotalIncome.Should().Be(totalIncome);
        result.TotalExpense.Should().Be(totalExpense);
        result.Balance.Should().Be(3250.00m);

        _reportRepositoryMock.Verify(
            repository => repository.GetFinancialTotalsAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Verifica se o saldo é calculado corretamente quando
    /// as receitas são maiores que as despesas.
    /// </summary>
    [Fact]
    public async Task GetFinancialSummaryAsync_ShouldCalculatePositiveBalance()
    {
        // Arrange
        _reportRepositoryMock
            .Setup(repository => repository.GetFinancialTotalsAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((
                TotalPeople: 2,
                TotalTransactions: 5,
                TotalIncome: 4200.00m,
                TotalExpense: 1200.00m));

        // Act
        var result = await _reportService.GetFinancialSummaryAsync();

        // Assert
        result.Balance.Should().Be(3000.00m);
    }

    /// <summary>
    /// Verifica se o serviço retorna saldo negativo quando
    /// as despesas são maiores que as receitas.
    /// </summary>
    [Fact]
    public async Task GetFinancialSummaryAsync_ShouldCalculateNegativeBalance()
    {
        // Arrange
        _reportRepositoryMock
            .Setup(repository => repository.GetFinancialTotalsAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((
                TotalPeople: 3,
                TotalTransactions: 7,
                TotalIncome: 1500.00m,
                TotalExpense: 2600.00m));

        // Act
        var result = await _reportService.GetFinancialSummaryAsync();

        // Assert
        result.TotalIncome.Should().Be(1500.00m);
        result.TotalExpense.Should().Be(2600.00m);
        result.Balance.Should().Be(-1100.00m);
    }

    /// <summary>
    /// Verifica se todos os indicadores permanecem zerados
    /// quando não existem pessoas ou transações cadastradas.
    /// </summary>
    [Fact]
    public async Task GetFinancialSummaryAsync_ShouldReturnZeroValues_WhenThereIsNoData()
    {
        // Arrange
        _reportRepositoryMock
            .Setup(repository => repository.GetFinancialTotalsAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((
                TotalPeople: 0,
                TotalTransactions: 0,
                TotalIncome: 0.00m,
                TotalExpense: 0.00m));

        // Act
        var result = await _reportService.GetFinancialSummaryAsync();

        // Assert
        result.TotalPeople.Should().Be(0);
        result.TotalTransactions.Should().Be(0);
        result.TotalIncome.Should().Be(0.00m);
        result.TotalExpense.Should().Be(0.00m);
        result.Balance.Should().Be(0.00m);
    }

    /// <summary>
    /// Verifica se o saldo é igual a zero quando
    /// receitas e despesas possuem o mesmo valor.
    /// </summary>
    [Fact]
    public async Task GetFinancialSummaryAsync_ShouldReturnZeroBalance_WhenTotalsAreEqual()
    {
        // Arrange
        _reportRepositoryMock
            .Setup(repository => repository.GetFinancialTotalsAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((
                TotalPeople: 2,
                TotalTransactions: 4,
                TotalIncome: 2000.00m,
                TotalExpense: 2000.00m));

        // Act
        var result = await _reportService.GetFinancialSummaryAsync();

        // Assert
        result.Balance.Should().Be(0.00m);
    }

    /// <summary>
    /// Verifica se o token de cancelamento recebido pelo serviço
    /// é encaminhado corretamente para o repositório.
    /// </summary>
    [Fact]
    public async Task GetFinancialSummaryAsync_ShouldForwardCancellationToken()
    {
        // Arrange
        using var cancellationTokenSource =
            new CancellationTokenSource();

        var cancellationToken =
            cancellationTokenSource.Token;

        _reportRepositoryMock
            .Setup(repository => repository.GetFinancialTotalsAsync(
                cancellationToken))
            .ReturnsAsync((
                TotalPeople: 1,
                TotalTransactions: 1,
                TotalIncome: 100.00m,
                TotalExpense: 25.00m));

        // Act
        var result = await _reportService.GetFinancialSummaryAsync(
            cancellationToken);

        // Assert
        result.Balance.Should().Be(75.00m);

        _reportRepositoryMock.Verify(
            repository => repository.GetFinancialTotalsAsync(
                cancellationToken),
            Times.Once);
    }

    /// <summary>
    /// Verifica se o repositório é consultado somente uma vez
    /// durante a geração do resumo financeiro.
    /// </summary>
    [Fact]
    public async Task GetFinancialSummaryAsync_ShouldCallRepositoryOnlyOnce()
    {
        // Arrange
        _reportRepositoryMock
            .Setup(repository => repository.GetFinancialTotalsAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((
                TotalPeople: 5,
                TotalTransactions: 12,
                TotalIncome: 8000.00m,
                TotalExpense: 3500.00m));

        // Act
        await _reportService.GetFinancialSummaryAsync();

        // Assert
        _reportRepositoryMock.Verify(
            repository => repository.GetFinancialTotalsAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);

        _reportRepositoryMock.VerifyNoOtherCalls();
    }
}