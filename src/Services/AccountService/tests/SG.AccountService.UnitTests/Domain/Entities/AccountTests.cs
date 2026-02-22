using FluentAssertions;
using SG.AccountService.Domain.Entities;
using SG.AccountService.Domain.Exceptions;
using SG.AccountService.Domain.ValueObjects;

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
        account.Balance.Should().Be(Money.Zero);
        account.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithEmptyUserId_ShouldThrowInvalidAccountException()
    {
        // Act
        Action act = () => new Account(Guid.Empty);

        // Assert
        act.Should().Throw<InvalidAccountException>()
            .WithMessage("El ID de usuario no puede ser vacío.");
    }

    [Theory]
    [InlineData(100)]
    [InlineData(0.01)]
    public void Deposit_WithValidAmount_ShouldIncreaseBalance(decimal depositAmount)
    {
        // Arrange
        var account = new Account(_validUserId);
        var amount = new Money(depositAmount);

        // Act
        account.Deposit(amount);

        // Assert
        account.Balance.Should().Be(amount);
    }

    [Fact]
    public void Deposit_WithZeroAmount_ShouldThrowInvalidAmountException()
    {
        // Arrange
        var account = new Account(_validUserId);

        // Act
        Action act = () => account.Deposit(Money.Zero);

        // Assert
        act.Should().Throw<InvalidMoneyException>()
            .WithMessage("El monto a depositar debe ser mayor a cero.");
    }

    [Fact]
    public void Withdraw_WithSufficientFunds_ShouldDecreaseBalance()
    {
        // Arrange
        var account = new Account(_validUserId);
        account.Deposit(new Money(500));

        // Act
        account.Withdraw(new Money(200));

        // Assert
        account.Balance.Should().Be(new Money(300));
    }

    [Fact]
    public void Withdraw_WithZeroAmount_ShouldThrowInvalidAmountException()
    {
        // Arrange
        var account = new Account(_validUserId);
        account.Deposit(new Money(500));

        // Act
        Action act = () => account.Withdraw(Money.Zero);

        // Assert
        act.Should().Throw<InvalidMoneyException>()
            .WithMessage("El monto a retirar debe ser mayor a cero.");
    }

    [Fact]
    public void Withdraw_WithInsufficientFunds_ShouldThrowInsufficientFundsException()
    {
        // Arrange
        var account = new Account(_validUserId);
        account.Deposit(new Money(100));

        // Act
        Action act = () => account.Withdraw(new Money(500));

        // Assert
        act.Should().Throw<InsufficientFundsException>();
    }
}