using SG.AccountService.Domain.Exceptions;
using SG.AccountService.Domain.ValueObjects;

namespace SG.AccountService.Domain.Entities;

public class Account
{
  public Guid Id { get; private set; }
  public Guid UserId { get; private set; }
  public Money Balance { get; private set; } = null!;
  public byte[] RowVersion { get; private set; } = null!;
  public DateTime CreatedAt { get; private set; }

  protected Account()
  {
  }

  public Account(Guid userId)
  {
    if (userId.Equals(Guid.Empty))
      throw new InvalidAccountException("El ID de usuario no puede ser vacío.");
    
    Id = Guid.NewGuid();
    UserId = userId;
    Balance = new Money(0);
    CreatedAt = DateTime.UtcNow;
  }
  
  public void Deposit(Money amount)
  {
    if (amount == Money.Zero)
      throw new InvalidMoneyException($"El monto a depositar debe ser mayor a cero.");
    
    Balance += amount;
  }

  public void Withdraw(Money amount)
  {
    if (amount == Money.Zero)
      throw new InvalidMoneyException($"El monto a retirar debe ser mayor a cero.");
    
    if (Balance < amount)
      throw new InsufficientFundsException();

    Balance -= amount;
  }
}