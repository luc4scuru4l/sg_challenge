using SG.AccountService.Domain.Enums;

namespace SG.AccountService.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public decimal Balance { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Account Account { get; private set; } = null!;

    protected Transaction() { }

    public Transaction(Guid accountId, TransactionType type, decimal amount, decimal balance)
    {
        Id = Guid.NewGuid();
        AccountId = accountId;
        Type = type;
        Amount = amount;
        Balance = balance;
        CreatedAt = DateTime.UtcNow;
    }
}