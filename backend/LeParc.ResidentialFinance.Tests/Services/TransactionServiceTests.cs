using System.Reflection;
using FluentAssertions;
using LeParc.ResidentialFinance.Application.DTOs.Transactions;
using LeParc.ResidentialFinance.Application.Exceptions;
using LeParc.ResidentialFinance.Application.Interfaces.Repositories;
using LeParc.ResidentialFinance.Application.Services;
using LeParc.ResidentialFinance.Domain.Entities;
using LeParc.ResidentialFinance.Domain.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace LeParc.ResidentialFinance.Tests.Services;

/// <summary>
/// Contém os testes unitários das regras de negócio implementadas
/// pelo serviço responsável pelas transações financeiras.
/// </summary>
public sealed class TransactionServiceTests
{
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<IPersonRepository> _personRepositoryMock;
    private readonly TransactionService _transactionService;

    /// <summary>
    /// Inicializa as dependências utilizadas em cada cenário de teste.
    /// </summary>
    public TransactionServiceTests()
    {
        _transactionRepositoryMock =
            new Mock<ITransactionRepository>();

        _personRepositoryMock =
            new Mock<IPersonRepository>();

        _transactionService = new TransactionService(
            _transactionRepositoryMock.Object,
            _personRepositoryMock.Object,
            NullLogger<TransactionService>.Instance);
    }

    /// <summary>
    /// Verifica se uma receita válida pode ser criada para uma pessoa adulta.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldCreateIncome_WhenPersonIsAdult()
    {
        // Arrange
        const int personId = 1;

        var person = CreatePersonWithId(
            personId,
            "Caíque Brandão",
            25);

        var request = new CreateTransactionRequest
        {
            Description = "Salário mensal",
            Amount = 3500.00m,
            Type = TransactionType.Income,
            PersonId = personId
        };

        FinancialTransaction? capturedTransaction = null;

        SetupPersonRepository(personId, person);

        _transactionRepositoryMock
            .Setup(repository => repository.AddAsync(
                It.IsAny<FinancialTransaction>(),
                It.IsAny<CancellationToken>()))
            .Callback<FinancialTransaction, CancellationToken>(
                (transaction, _) =>
                    capturedTransaction = transaction)
            .Returns(Task.CompletedTask);

        SetupSaveChanges();

        // Act
        var result = await _transactionService.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be("Salário mensal");
        result.Amount.Should().Be(3500.00m);
        result.Type.Should().Be(TransactionType.Income);
        result.PersonId.Should().Be(personId);
        result.PersonName.Should().Be("Caíque Brandão");

        capturedTransaction.Should().NotBeNull();
        capturedTransaction!.Description.Should().Be("Salário mensal");
        capturedTransaction.Amount.Should().Be(3500.00m);
        capturedTransaction.Type.Should().Be(TransactionType.Income);
        capturedTransaction.Person.Should().BeSameAs(person);

        VerifySuccessfulCreation(personId);
    }

    /// <summary>
    /// Verifica se uma despesa válida pode ser criada para uma pessoa adulta.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldCreateExpense_WhenPersonIsAdult()
    {
        // Arrange
        const int personId = 2;

        var person = CreatePersonWithId(
            personId,
            "Ana Souza",
            32);

        var request = new CreateTransactionRequest
        {
            Description = "Conta de energia",
            Amount = 245.90m,
            Type = TransactionType.Expense,
            PersonId = personId
        };

        FinancialTransaction? capturedTransaction = null;

        SetupPersonRepository(personId, person);

        _transactionRepositoryMock
            .Setup(repository => repository.AddAsync(
                It.IsAny<FinancialTransaction>(),
                It.IsAny<CancellationToken>()))
            .Callback<FinancialTransaction, CancellationToken>(
                (transaction, _) =>
                    capturedTransaction = transaction)
            .Returns(Task.CompletedTask);

        SetupSaveChanges();

        // Act
        var result = await _transactionService.CreateAsync(request);

        // Assert
        result.Description.Should().Be("Conta de energia");
        result.Amount.Should().Be(245.90m);
        result.Type.Should().Be(TransactionType.Expense);
        result.PersonId.Should().Be(personId);
        result.PersonName.Should().Be("Ana Souza");

        capturedTransaction.Should().NotBeNull();
        capturedTransaction!.Type.Should().Be(
            TransactionType.Expense);

        VerifySuccessfulCreation(personId);
    }

    /// <summary>
    /// Verifica se uma pessoa menor de idade pode cadastrar uma despesa.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldCreateExpense_WhenPersonIsMinor()
    {
        // Arrange
        const int personId = 3;

        var person = CreatePersonWithId(
            personId,
            "Pedro Santos",
            17);

        var request = new CreateTransactionRequest
        {
            Description = "Material escolar",
            Amount = 150.00m,
            Type = TransactionType.Expense,
            PersonId = personId
        };

        SetupPersonRepository(personId, person);
        SetupTransactionCreation();

        // Act
        var result = await _transactionService.CreateAsync(request);

        // Assert
        result.Description.Should().Be("Material escolar");
        result.Amount.Should().Be(150.00m);
        result.Type.Should().Be(TransactionType.Expense);
        result.PersonId.Should().Be(personId);
        result.PersonName.Should().Be("Pedro Santos");

        VerifySuccessfulCreation(personId);
    }

    /// <summary>
    /// Verifica se uma pessoa menor de idade não pode cadastrar receita.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenMinorCreatesIncome()
    {
        // Arrange
        const int personId = 4;

        var person = CreatePersonWithId(
            personId,
            "Lucas Oliveira",
            16);

        var request = new CreateTransactionRequest
        {
            Description = "Pagamento recebido",
            Amount = 500.00m,
            Type = TransactionType.Income,
            PersonId = personId
        };

        SetupPersonRepository(personId, person);

        // Act
        var action = async () =>
            await _transactionService.CreateAsync(request);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage(
                "Pessoas menores de 18 anos só podem registrar despesas.");

        _personRepositoryMock.Verify(
            repository => repository.GetByIdAsync(
                personId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        VerifyTransactionWasNotPersisted();
    }

    /// <summary>
    /// Verifica se a tentativa de criar uma transação para uma pessoa
    /// inexistente gera uma exceção de recurso não encontrado.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldThrowNotFoundException_WhenPersonDoesNotExist()
    {
        // Arrange
        const int personId = 999;

        var request = new CreateTransactionRequest
        {
            Description = "Conta de internet",
            Amount = 120.00m,
            Type = TransactionType.Expense,
            PersonId = personId
        };

        _personRepositoryMock
            .Setup(repository => repository.GetByIdAsync(
                personId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Person?)null);

        // Act
        var action = async () =>
            await _transactionService.CreateAsync(request);

        // Assert
        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("A pessoa informada não foi encontrada.");

        _personRepositoryMock.Verify(
            repository => repository.GetByIdAsync(
                personId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        VerifyTransactionWasNotPersisted();
    }

    /// <summary>
    /// Verifica se uma requisição nula é rejeitada antes de qualquer
    /// consulta ou operação de persistência.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        CreateTransactionRequest request = null!;

        // Act
        var action = async () =>
            await _transactionService.CreateAsync(request);

        // Assert
        await action.Should()
            .ThrowAsync<ArgumentNullException>()
            .WithParameterName("request");

        VerifyPersonWasNotConsulted();
        VerifyTransactionWasNotPersisted();
    }

    /// <summary>
    /// Verifica se identificadores de pessoa inválidos são rejeitados.
    /// </summary>
    /// <param name="invalidPersonId">
    /// Identificador inválido utilizado no cenário.
    /// </param>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task CreateAsync_ShouldThrowArgumentOutOfRangeException_WhenPersonIdIsInvalid(
        int invalidPersonId)
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            Description = "Conta residencial",
            Amount = 100.00m,
            Type = TransactionType.Expense,
            PersonId = invalidPersonId
        };

        // Act
        var action = async () =>
            await _transactionService.CreateAsync(request);

        // Assert
        await action.Should()
            .ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("PersonId")
            .WithMessage(
                "*O identificador da pessoa deve ser maior que zero.*");

        VerifyPersonWasNotConsulted();
        VerifyTransactionWasNotPersisted();
    }

    /// <summary>
    /// Verifica se valores monetários menores ou iguais a zero são rejeitados.
    /// </summary>
    /// <param name="invalidAmount">
    /// Valor monetário inválido utilizado no cenário.
    /// </param>
    [Theory]
    [InlineData("0")]
    [InlineData("-0.01")]
    [InlineData("-100")]
    public async Task CreateAsync_ShouldThrowArgumentOutOfRangeException_WhenAmountIsInvalid(
        string invalidAmount)
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            Description = "Transação inválida",
            Amount = decimal.Parse(
                invalidAmount,
                System.Globalization.CultureInfo.InvariantCulture),
            Type = TransactionType.Expense,
            PersonId = 1
        };

        // Act
        var action = async () =>
            await _transactionService.CreateAsync(request);

        // Assert
        await action.Should()
            .ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("Amount")
            .WithMessage(
                "*O valor da transação deve ser maior que zero.*");

        VerifyPersonWasNotConsulted();
        VerifyTransactionWasNotPersisted();
    }

    /// <summary>
    /// Verifica se valores que não pertencem ao enum de tipos de
    /// transação são rejeitados.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldThrowArgumentException_WhenTransactionTypeIsInvalid()
    {
        // Arrange
        var request = new CreateTransactionRequest
        {
            Description = "Transação inválida",
            Amount = 100.00m,
            Type = (TransactionType)999,
            PersonId = 1
        };

        // Act
        var action = async () =>
            await _transactionService.CreateAsync(request);

        // Assert
        await action.Should()
            .ThrowAsync<ArgumentException>()
            .WithParameterName("Type")
            .WithMessage("*O tipo da transação é inválido.*");

        VerifyPersonWasNotConsulted();
        VerifyTransactionWasNotPersisted();
    }

    /// <summary>
    /// Verifica se uma descrição vazia ou composta apenas por espaços
    /// é rejeitada.
    /// </summary>
    /// <param name="invalidDescription">
    /// Descrição inválida utilizada no cenário.
    /// </param>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("     ")]
    public async Task CreateAsync_ShouldThrowArgumentException_WhenDescriptionIsInvalid(
        string invalidDescription)
    {
        // Arrange
        const int personId = 5;

        var person = CreatePersonWithId(
            personId,
            "Mariana Lima",
            28);

        var request = new CreateTransactionRequest
        {
            Description = invalidDescription,
            Amount = 100.00m,
            Type = TransactionType.Expense,
            PersonId = personId
        };

        SetupPersonRepository(personId, person);

        // Act
        var action = async () =>
            await _transactionService.CreateAsync(request);

        // Assert
        await action.Should()
            .ThrowAsync<ArgumentException>()
            .WithParameterName("description")
            .WithMessage(
                "*A descrição da transação é obrigatória.*");

        _personRepositoryMock.Verify(
            repository => repository.GetByIdAsync(
                personId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        VerifyTransactionWasNotPersisted();
    }

    /// <summary>
    /// Verifica se espaços desnecessários são removidos da descrição
    /// antes da criação da transação.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldNormalizeDescription_WhenDescriptionHasExtraSpaces()
    {
        // Arrange
        const int personId = 6;

        var person = CreatePersonWithId(
            personId,
            "Roberto Almeida",
            40);

        var request = new CreateTransactionRequest
        {
            Description = "   Compra    mensal   de    alimentos   ",
            Amount = 850.00m,
            Type = TransactionType.Expense,
            PersonId = personId
        };

        FinancialTransaction? capturedTransaction = null;

        SetupPersonRepository(personId, person);

        _transactionRepositoryMock
            .Setup(repository => repository.AddAsync(
                It.IsAny<FinancialTransaction>(),
                It.IsAny<CancellationToken>()))
            .Callback<FinancialTransaction, CancellationToken>(
                (transaction, _) =>
                    capturedTransaction = transaction)
            .Returns(Task.CompletedTask);

        SetupSaveChanges();

        // Act
        var result = await _transactionService.CreateAsync(request);

        // Assert
        const string normalizedDescription =
            "Compra mensal de alimentos";

        result.Description.Should().Be(normalizedDescription);

        capturedTransaction.Should().NotBeNull();
        capturedTransaction!.Description.Should().Be(
            normalizedDescription);

        _transactionRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.Is<FinancialTransaction>(
                    transaction =>
                        transaction.Description ==
                        normalizedDescription),
                It.IsAny<CancellationToken>()),
            Times.Once);

        VerifySuccessfulCreation(personId);
    }

    /// <summary>
    /// Verifica se todas as transações são retornadas e mapeadas
    /// corretamente.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTransactions()
    {
        // Arrange
        var firstPerson = CreatePersonWithId(
            1,
            "Caíque Brandão",
            25);

        var secondPerson = CreatePersonWithId(
            2,
            "Ana Souza",
            32);

        var transactions = new List<FinancialTransaction>
        {
            new(
                "Salário mensal",
                3500.00m,
                TransactionType.Income,
                firstPerson),

            new(
                "Conta de energia",
                245.90m,
                TransactionType.Expense,
                secondPerson)
        };

        _transactionRepositoryMock
            .Setup(repository => repository.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactions);

        // Act
        var result = await _transactionService.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);

        result[0].Description.Should().Be("Salário mensal");
        result[0].Amount.Should().Be(3500.00m);
        result[0].Type.Should().Be(TransactionType.Income);
        result[0].PersonId.Should().Be(1);
        result[0].PersonName.Should().Be("Caíque Brandão");

        result[1].Description.Should().Be("Conta de energia");
        result[1].Amount.Should().Be(245.90m);
        result[1].Type.Should().Be(TransactionType.Expense);
        result[1].PersonId.Should().Be(2);
        result[1].PersonName.Should().Be("Ana Souza");

        _transactionRepositoryMock.Verify(
            repository => repository.GetAllAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);

        _transactionRepositoryMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// Configura o retorno de uma pessoa pelo repositório.
    /// </summary>
    /// <param name="personId">Identificador consultado.</param>
    /// <param name="person">Pessoa que será retornada.</param>
    private void SetupPersonRepository(
        int personId,
        Person person)
    {
        _personRepositoryMock
            .Setup(repository => repository.GetByIdAsync(
                personId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);
    }

    /// <summary>
    /// Configura as operações necessárias para persistir uma transação.
    /// </summary>
    private void SetupTransactionCreation()
    {
        _transactionRepositoryMock
            .Setup(repository => repository.AddAsync(
                It.IsAny<FinancialTransaction>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        SetupSaveChanges();
    }

    /// <summary>
    /// Configura a confirmação das alterações realizadas no repositório.
    /// </summary>
    private void SetupSaveChanges()
    {
        _transactionRepositoryMock
            .Setup(repository => repository.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    /// <summary>
    /// Verifica se todas as operações esperadas durante uma criação
    /// bem-sucedida foram executadas exatamente uma vez.
    /// </summary>
    /// <param name="personId">
    /// Identificador da pessoa consultada durante a operação.
    /// </param>
    private void VerifySuccessfulCreation(int personId)
    {
        _personRepositoryMock.Verify(
            repository => repository.GetByIdAsync(
                personId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _transactionRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.IsAny<FinancialTransaction>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _transactionRepositoryMock.Verify(
            repository => repository.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Verifica se nenhuma consulta ao repositório de pessoas foi realizada.
    /// </summary>
    private void VerifyPersonWasNotConsulted()
    {
        _personRepositoryMock.Verify(
            repository => repository.GetByIdAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica se nenhuma operação de persistência de transação foi realizada.
    /// </summary>
    private void VerifyTransactionWasNotPersisted()
    {
        _transactionRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.IsAny<FinancialTransaction>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _transactionRepositoryMock.Verify(
            repository => repository.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Cria uma pessoa para os testes e atribui um identificador,
    /// simulando uma entidade já persistida no banco de dados.
    /// </summary>
    /// <param name="id">Identificador que será atribuído.</param>
    /// <param name="name">Nome da pessoa.</param>
    /// <param name="age">Idade da pessoa.</param>
    /// <returns>Uma pessoa preparada para o cenário de teste.</returns>
    private static Person CreatePersonWithId(
        int id,
        string name,
        int age)
    {
        var person = new Person(name, age);

        var idProperty = typeof(Person).GetProperty(
            nameof(Person.Id),
            BindingFlags.Instance | BindingFlags.Public);

        idProperty.Should().NotBeNull(
            "a entidade Person deve possuir a propriedade Id");

        idProperty!.SetValue(person, id);

        return person;
    }
}