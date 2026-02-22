using FluentAssertions;
using SG.AccountService.Domain.Exceptions;
using SG.AccountService.Domain.ValueObjects;

namespace SG.AccountService.UnitTests.Domain.ValueObjects;

public class MoneyTests
{
  [Theory]
  [InlineData(0)]
  [InlineData(100)]
  [InlineData(99.99)]
  public void Constructor_WithValidAmount_ShouldCreateInstance(decimal validAmount)
  {
    // Act
    var money = new Money(validAmount);

    // Assert
    money.Value.Should().Be(validAmount);
  }

  [Theory]
  [InlineData(-0.01)]
  [InlineData(-50)]
  public void Constructor_WithNegativeAmount_ShouldThrowInvalidMoneyException(decimal negativeAmount)
  {
    // Act
    Action act = () => new Money(negativeAmount);

    // Assert
    // Asumiendo que usaste InvalidMoneyException o InvalidAmountException en tu record Money
    act.Should().Throw<InvalidMoneyException>()
      .WithMessage("El monto no puede ser negativo.");
  }

  [Theory]
  [InlineData(100.123)]
  [InlineData(50.5555)]
  public void Constructor_WithMoreThanTwoDecimals_ShouldThrowInvalidMoneyException(decimal invalidAmount)
  {
    // Act
    Action act = () => new Money(invalidAmount);

    // Assert
    act.Should().Throw<InvalidMoneyException>()
      .WithMessage("El monto no puede tener más de dos decimales.");
  }
}