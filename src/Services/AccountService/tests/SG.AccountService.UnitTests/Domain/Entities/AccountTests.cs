using FluentAssertions;
using SG.AccountService.Domain.Entities;

namespace SG.AccountService.UnitTests.Domain.Entities;

public class AccountTests
{
    private readonly Guid _validUserId = Guid.NewGuid();

    [Fact]
    public void Constructor_ShouldInitializeAccount_WithZeroBalance()
    {
        // Arrange & Act
        var account = new Account(_validUserId);

        // Assert
        account.Id.Should().NotBeEmpty();
        account.UserId.Should().Be(_validUserId);
        account.Balance.Should().Be(0);
        account.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(100)]
    [InlineData(0.01)]
    public void Deposit_WithValidAmount_ShouldIncreaseBalance(decimal depositAmount)
    {
        // Arrange
        var account = new Account(_validUserId);

        // Act
        account.Deposit(depositAmount);

        // Assert
        account.Balance.Should().Be(depositAmount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public void Deposit_WithZeroOrNegativeAmount_ShouldThrowArgumentException(decimal invalidAmount)
    {
        // Arrange
        var account = new Account(_validUserId);

        // Act
        Action act = () => account.Deposit(invalidAmount);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("El monto a depositar debe ser mayor a cero.");
    }

    [Fact]
    public void Withdraw_WithSufficientFunds_ShouldDecreaseBalance()
    {
        // Arrange
        var account = new Account(_validUserId);
        account.Deposit(500);

        // Act
        account.Withdraw(200);

        // Assert
        account.Balance.Should().Be(300);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Withdraw_WithZeroOrNegativeAmount_ShouldThrowArgumentException(decimal invalidAmount)
    {
        // Arrange
        var account = new Account(_validUserId);
        account.Deposit(500);

        // Act
        Action act = () => account.Withdraw(invalidAmount);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("El monto a retirar debe ser mayor a cero.");
    }

    [Fact]
    public void Withdraw_WithInsufficientFunds_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var account = new Account(_validUserId);
        account.Deposit(100);

        // Act
        Action act = () => account.Withdraw(500);

        // Assert
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Fondos insuficientes.");
    }
}