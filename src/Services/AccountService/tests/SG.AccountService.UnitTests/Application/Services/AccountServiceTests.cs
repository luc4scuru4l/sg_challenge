using FluentAssertions;
using Moq;
using SG.AccountService.Application.DTOs;
using SG.AccountService.Application.Exceptions;
using AccountServiceClass = SG.AccountService.Application.Services.AccountService;
using SG.AccountService.Domain.Entities;
using SG.AccountService.Domain.Enums;
using SG.AccountService.Domain.Exceptions;
using SG.AccountService.Domain.Repositories;
using SG.AccountService.Domain.ValueObjects;

namespace SG.AccountService.UnitTests.Application.Services;

public class AccountServiceTests
{
  private readonly Mock<IAccountRepository> _repositoryMock;
  private readonly AccountServiceClass _sut;
  private readonly Guid _userId;
  private readonly Guid _accountId;

  public AccountServiceTests()
  {
    _repositoryMock = new Mock<IAccountRepository>();
    _sut = new AccountServiceClass(_repositoryMock.Object);
    _userId = Guid.NewGuid();
    _accountId = Guid.NewGuid();
  }

  [Fact]
  public async Task CreateAccountAsync_ShouldCreateAccountAndReturnDto()
  {
    // Act
    var result = await _sut.CreateAccountAsync(_userId);

    // Assert
    result.Should().NotBeNull();
    result.Balance.Should().Be(0);
    _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Once);
  }

  [Fact]
  public async Task GetBalanceAsync_WhenAccountExists_ShouldReturnBalance()
  {
    // Arrange
    var account = new Account(_userId);
    account.Deposit(new Money(1500));

    _repositoryMock.Setup(x => x.GetByIdAndUserIdAsync(It.IsAny<Guid>(), _userId, It.IsAny<CancellationToken>()))
      .ReturnsAsync(account);

    // Act
    var result = await _sut.GetBalanceAsync(account.Id, _userId);

    // Assert
    result.Balance.Should().Be(1500);
  }

  [Fact]
  public async Task GetBalanceAsync_WhenAccountDoesNotExist_ShouldThrowAccountNotFoundException()
  {
    // Arrange
    _repositoryMock.Setup(x =>
        x.GetByIdAndUserIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync((Account)null!);

    // Act
    Func<Task> act = async () => await _sut.GetBalanceAsync(_accountId, _userId);

    // Assert
    await act.Should().ThrowAsync<AccountNotFoundException>();
  }

  [Fact]
  public async Task DepositAsync_WithValidAmount_ShouldUpdateBalanceAndSaveTransaction()
  {
    // Arrange
    var account = new Account(_userId);
    _repositoryMock.Setup(x => x.GetByIdAndUserIdAsync(It.IsAny<Guid>(), _userId, It.IsAny<CancellationToken>()))
      .ReturnsAsync(account);

    // Act
    var result = await _sut.DepositAsync(account.Id, _userId, 500);

    // Assert
    result.Balance.Should().Be(500);
    _repositoryMock.Verify(x => x.UpdateWithTransactionAsync(
      account,
      It.Is<Transaction>(t => t.Type == TransactionType.Deposit && t.Amount == 500),
      It.IsAny<CancellationToken>()), Times.Once);
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-100)]
  public async Task DepositAsync_WithZeroOrNegativeAmount_ShouldFailFastAndThrowInvalidMoneyException(
    decimal invalidAmount)
  {
    // Act
    Func<Task> act = async () => await _sut.DepositAsync(_accountId, _userId, invalidAmount);

    // Assert
    await act.Should().ThrowAsync<InvalidMoneyException>();
    _repositoryMock.Verify(
      x => x.GetByIdAndUserIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact]
  public async Task WithdrawAsync_WithValidAmount_ShouldUpdateBalanceAndSaveTransaction()
  {
    // Arrange
    var account = new Account(_userId);
    account.Deposit(new Money(1000));

    _repositoryMock.Setup(x => x.GetByIdAndUserIdAsync(It.IsAny<Guid>(), _userId, It.IsAny<CancellationToken>()))
      .ReturnsAsync(account);

    // Act
    var result = await _sut.WithdrawAsync(account.Id, _userId, 400);

    // Assert
    result.Balance.Should().Be(600);
    _repositoryMock.Verify(x => x.UpdateWithTransactionAsync(
      account,
      It.Is<Transaction>(t => t.Type == TransactionType.Withdrawal && t.Amount == 400),
      It.IsAny<CancellationToken>()), Times.Once);
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-50)]
  public async Task WithdrawAsync_WithZeroOrNegativeAmount_ShouldFailFastAndThrowInvalidMoneyException(
    decimal invalidAmount)
  {
    // Act
    Func<Task> act = async () => await _sut.WithdrawAsync(_accountId, _userId, invalidAmount);

    // Assert
    await act.Should().ThrowAsync<InvalidMoneyException>();
    _repositoryMock.Verify(
      x => x.GetByIdAndUserIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact]
  public async Task WithdrawAsync_WithInsufficientFunds_ShouldThrowInsufficientFundsException()
  {
    // Arrange
    var account = new Account(_userId);
    account.Deposit(new Money(100));

    _repositoryMock.Setup(x => x.GetByIdAndUserIdAsync(It.IsAny<Guid>(), _userId, It.IsAny<CancellationToken>()))
      .ReturnsAsync(account);

    // Act
    Func<Task> act = async () => await _sut.WithdrawAsync(account.Id, _userId, 500); // Intenta retirar 500

    // Assert
    await act.Should().ThrowAsync<InsufficientFundsException>();
    _repositoryMock.Verify(
      x => x.UpdateWithTransactionAsync(It.IsAny<Account>(), It.IsAny<Transaction>(), It.IsAny<CancellationToken>()),
      Times.Never);
  }

  [Fact]
  public async Task DepositAsync_WhenConcurrencyConflictOccurs_ShouldThrowConcurrencyConflictException()
  {
    // Arrange
    var account = new Account(_userId);
    _repositoryMock.Setup(x => x.GetByIdAndUserIdAsync(It.IsAny<Guid>(), _userId, It.IsAny<CancellationToken>()))
      .ReturnsAsync(account);

    // Simulamos que al intentar guardar, otro hilo ya modificó la fila
    _repositoryMock.Setup(x =>
        x.UpdateWithTransactionAsync(It.IsAny<Account>(), It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
      .ThrowsAsync(new ConcurrencyConflictException());

    // Act
    Func<Task> act = async () => await _sut.DepositAsync(account.Id, _userId, 500);

    // Assert
    await act.Should().ThrowAsync<ConcurrencyConflictException>();
  }
}