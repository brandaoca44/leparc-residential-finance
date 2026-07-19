using FluentAssertions;
using LeParc.ResidentialFinance.Application.DTOs.People;
using LeParc.ResidentialFinance.Application.Exceptions;
using LeParc.ResidentialFinance.Application.Interfaces.Repositories;
using LeParc.ResidentialFinance.Application.Services;
using LeParc.ResidentialFinance.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace LeParc.ResidentialFinance.Tests.Services;

/// <summary>
/// Testes unitários das regras de negócio do serviço de pessoas.
/// </summary>
public sealed class PersonServiceTests
{
    private readonly Mock<IPersonRepository> _personRepositoryMock;
    private readonly PersonService _personService;

    /// <summary>
    /// Inicializa as dependências utilizadas em cada teste.
    /// </summary>
    public PersonServiceTests()
    {
        _personRepositoryMock = new Mock<IPersonRepository>();

        _personService = new PersonService(
            _personRepositoryMock.Object,
            NullLogger<PersonService>.Instance);
    }

    /// <summary>
    /// Verifica se uma pessoa válida é criada e persistida corretamente.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldCreatePerson_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreatePersonRequest
        {
            Name = "Caíque Brandão",
            Age = 25
        };

        Person? capturedPerson = null;

        _personRepositoryMock
            .Setup(repository => repository.AddAsync(
                It.IsAny<Person>(),
                It.IsAny<CancellationToken>()))
            .Callback<Person, CancellationToken>(
                (person, _) => capturedPerson = person)
            .Returns(Task.CompletedTask);

        _personRepositoryMock
            .Setup(repository => repository.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _personService.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Caíque Brandão");
        result.Age.Should().Be(25);

        capturedPerson.Should().NotBeNull();
        capturedPerson!.Name.Should().Be("Caíque Brandão");
        capturedPerson.Age.Should().Be(25);

        _personRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.IsAny<Person>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _personRepositoryMock.Verify(
            repository => repository.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Verifica se espaços excedentes são removidos do nome.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldNormalizeName_WhenNameHasExtraSpaces()
    {
        // Arrange
        var request = new CreatePersonRequest
        {
            Name = "   Caíque    César   Brandão   ",
            Age = 25
        };

        Person? capturedPerson = null;

        _personRepositoryMock
            .Setup(repository => repository.AddAsync(
                It.IsAny<Person>(),
                It.IsAny<CancellationToken>()))
            .Callback<Person, CancellationToken>(
                (person, _) => capturedPerson = person)
            .Returns(Task.CompletedTask);

        _personRepositoryMock
            .Setup(repository => repository.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _personService.CreateAsync(request);

        // Assert
        result.Name.Should().Be("Caíque César Brandão");

        capturedPerson.Should().NotBeNull();
        capturedPerson!.Name.Should().Be("Caíque César Brandão");

        _personRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.Is<Person>(
                    person => person.Name == "Caíque César Brandão"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Verifica se uma requisição nula é rejeitada.
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        CreatePersonRequest request = null!;

        // Act
        var action = async () => await _personService.CreateAsync(request);

        // Assert
        await action.Should()
            .ThrowAsync<ArgumentNullException>()
            .WithParameterName("request");

        _personRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.IsAny<Person>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _personRepositoryMock.Verify(
            repository => repository.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica se todas as pessoas são retornadas e mapeadas corretamente.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPeople()
    {
        // Arrange
        IReadOnlyList<Person> people =
        [
            new Person("Ana Souza", 32),
            new Person("Pedro Santos", 17)
        ];

        _personRepositoryMock
            .Setup(repository => repository.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(people);

        // Act
        var result = await _personService.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);

        result[0].Name.Should().Be("Ana Souza");
        result[0].Age.Should().Be(32);

        result[1].Name.Should().Be("Pedro Santos");
        result[1].Age.Should().Be(17);

        _personRepositoryMock.Verify(
            repository => repository.GetAllAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Verifica se uma pessoa existente é excluída corretamente.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldDeletePerson_WhenPersonExists()
    {
        // Arrange
        const int personId = 1;

        var person = new Person("Caíque Brandão", 25);

        _personRepositoryMock
            .Setup(repository => repository.GetByIdAsync(
                personId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        _personRepositoryMock
            .Setup(repository => repository.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _personService.DeleteAsync(personId);

        // Assert
        _personRepositoryMock.Verify(
            repository => repository.GetByIdAsync(
                personId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _personRepositoryMock.Verify(
            repository => repository.Remove(person),
            Times.Once);

        _personRepositoryMock.Verify(
            repository => repository.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Verifica se a exclusão de uma pessoa inexistente gera erro.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenPersonDoesNotExist()
    {
        // Arrange
        const int personId = 99;

        _personRepositoryMock
            .Setup(repository => repository.GetByIdAsync(
                personId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Person?)null);

        // Act
        var action = async () =>
            await _personService.DeleteAsync(personId);

        // Assert
        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("A pessoa informada não foi encontrada.");

        _personRepositoryMock.Verify(
            repository => repository.GetByIdAsync(
                personId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _personRepositoryMock.Verify(
            repository => repository.Remove(
                It.IsAny<Person>()),
            Times.Never);

        _personRepositoryMock.Verify(
            repository => repository.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Verifica se identificadores inválidos são rejeitados.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task DeleteAsync_ShouldThrowArgumentOutOfRangeException_WhenIdIsInvalid(
        int invalidId)
    {
        // Act
        var action = async () =>
            await _personService.DeleteAsync(invalidId);

        // Assert
        await action.Should()
            .ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("id")
            .WithMessage(
                "*O identificador deve ser maior que zero.*");

        _personRepositoryMock.Verify(
            repository => repository.GetByIdAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _personRepositoryMock.Verify(
            repository => repository.Remove(
                It.IsAny<Person>()),
            Times.Never);

        _personRepositoryMock.Verify(
            repository => repository.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}