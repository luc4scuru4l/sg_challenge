namespace SG.AccountService.Domain.Entities;

public class Account
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Balance { get; private set; }
    public byte[] RowVersion { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    
    protected Account() { }

    public Account(Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Balance = 0;
        CreatedAt = DateTime.UtcNow;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("El monto a depositar debe ser mayor a cero.");
        
        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("El monto a retirar debe ser mayor a cero.");
        if (Balance < amount) throw new InvalidOperationException("Fondos insuficientes.");
        
        Balance -= amount;
    }
}