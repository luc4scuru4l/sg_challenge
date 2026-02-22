using FluentAssertions;
using Moq;
using SG.AccountService.Application.DTOs;
using SG.AccountService.Application.Exceptions;
using SG.AccountService.Domain.Repositories;
using AccountServiceClass = SG.AccountService.Application.Services.AccountService;
using SG.AccountService.Domain.Entities;

namespace SG.AccountService.UnitTests.Application.Services;

public class AccountServiceTests
{
  private readonly Mock<IAccountRepository> _accountRepositoryMock;
  private readonly AccountServiceClass _sut;

  public AccountServiceTests()
  {
    _accountRepositoryMock = new Mock<IAccountRepository>();
    _sut = new AccountServiceClass(_accountRepositoryMock.Object);
  }

  [Fact]
  public async Task GetBalanceAsync_WhenAccountExistsAndBelongsToUser_ShouldReturnBalance()
  {
    // Arrange (Preparar)
    var accountId = Guid.NewGuid();
    var requestUserId = Guid.NewGuid();
    var expectedBalance = 15000.50m;

    // Simulamos una cuenta válida creada por el usuario
    var fakeAccount = new Account(requestUserId);
    fakeAccount.Deposit(expectedBalance);

    _accountRepositoryMock
      .Setup(repo => repo.GetByIdAndUserIdAsync(accountId, requestUserId, It.IsAny<CancellationToken>()))
      .ReturnsAsync(fakeAccount);

    // Act (Actuar)
    var result = await _sut.GetBalanceAsync(accountId, requestUserId, CancellationToken.None);

    // Assert (Afirmar)
    result.Should().NotBeNull();
    result.Should().BeOfType<AccountResponseDto>();
    result.AccountId.Should().Be(fakeAccount.Id);
    result.Balance.Should().Be(expectedBalance);

    // Verificamos que el servicio efectivamente llamó a la base de datos exactamente 1 vez
    _accountRepositoryMock.Verify(repo =>
        repo.GetByIdAndUserIdAsync(accountId, requestUserId, It.IsAny<CancellationToken>()),
      Times.Once);
  }

  [Fact]
  public async Task GetBalanceAsync_WhenAccountDoesNotExistOrBelongsToAnotherUser_ShouldThrowNotFoundException()
  {
    // Arrange
    var accountId = Guid.NewGuid();
    var hackerUserId = Guid.NewGuid();

    // Le decimos al Mock: "Devolvé NULL simulando que el filtro SQL no encontró coincidencias"
    _accountRepositoryMock
      .Setup(repo => repo.GetByIdAndUserIdAsync(accountId, hackerUserId, It.IsAny<CancellationToken>()))
      .ReturnsAsync((Account)null!);

    // Act
    Func<Task> act = async () => await _sut.GetBalanceAsync(accountId, hackerUserId, CancellationToken.None);

    // Assert
    await act.Should().ThrowAsync<AccountNotFoundException>()
      .WithMessage($"No se encontró la cuenta con el ID especificado: {accountId}");
  }
}