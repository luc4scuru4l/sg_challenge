using Microsoft.EntityFrameworkCore;
using SG.AccountService.Application.Interfaces;
using SG.AccountService.Domain.Entities;
using SG.AccountService.Infrastructure.Data;

namespace SG.AccountService.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AccountDbContext _context;

    public AccountRepository(AccountDbContext context)
    {
        _context = context;
    }

    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        await _context.Accounts.AddAsync(account, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateWithTransactionAsync(Account account, Transaction transaction, CancellationToken cancellationToken = default)
    {
        await _context.Transactions.AddAsync(transaction, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}