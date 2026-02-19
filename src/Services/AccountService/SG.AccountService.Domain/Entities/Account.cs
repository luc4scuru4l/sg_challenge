using SG.AccountService.Domain.Exceptions;

namespace SG.AccountService.Domain.Entities;

public class Account
{
  public Guid Id { get; private set; }
  public Guid UserId { get; private set; }
  public decimal Balance { get; private set; }
  public byte[] RowVersion { get; private set; } = null!;
  public DateTime CreatedAt { get; private set; }

  protected Account()
  {
  }

  public Account(Guid userId)
  {
    Id = Guid.NewGuid();
    UserId = userId;
    Balance = 0;
    CreatedAt = DateTime.UtcNow;
  }

  private void ValidateAmount(decimal amount, string action)
  {
    if (amount <= 0)
      throw new InvalidAmountException($"El monto a {action} debe ser mayor a cero.");

    if (Math.Round(amount, 2) != amount)
      throw new InvalidAmountException("El monto de la transacción no puede tener más de 2 decimales.");
  }
  
  public void Deposit(decimal amount)
  {
    ValidateAmount(amount, "depositar");
    Balance += amount;
  }

  public void Withdraw(decimal amount)
  {
    ValidateAmount(amount, "retirar");
    if (Balance < amount)
      throw new InsufficientFundsException();

    Balance -= amount;
  }
}