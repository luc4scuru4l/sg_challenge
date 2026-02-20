using SG.AccountService.Application.DTOs;
using SG.AccountService.Application.Exceptions;
using SG.AccountService.Application.Interfaces;
using SG.AccountService.Domain.Entities;
using SG.AccountService.Domain.Enums;

namespace SG.AccountService.Application.Services;

public class AccountService : IAccountService
{
  private readonly IAccountRepository _repository;

  public AccountService(IAccountRepository repository)
  {
    _repository = repository;
  }

  public async Task<AccountResponseDto> CreateAccountAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    var account = new Account(userId);

    await _repository.AddAsync(account, cancellationToken);

    return new AccountResponseDto(account.Id, account.Balance);
  }

  public async Task<AccountResponseDto> GetBalanceAsync(Guid accountId, Guid userId, CancellationToken cancellationToken = default)
  {
    var account = await GetAccountOrThrowAsync(accountId, userId, cancellationToken);
    return new AccountResponseDto(account.Id, account.Balance);
  }

  public async Task<AccountResponseDto> DepositAsync(Guid accountId, Guid userId, decimal amount, CancellationToken cancellationToken = default)
  {
    var account = await GetAccountOrThrowAsync(accountId, userId, cancellationToken);
    account.Deposit(amount);

    var transaction = new Transaction(account.Id, TransactionType.Deposit, amount, account.Balance);
    await _repository.UpdateWithTransactionAsync(account, transaction, cancellationToken);
    
    return new AccountResponseDto(account.Id, account.Balance);
  }

  public async Task<AccountResponseDto> WithdrawAsync(Guid accountId, Guid userId, decimal amount, CancellationToken cancellationToken = default)
  {
    var account = await GetAccountOrThrowAsync(accountId, userId, cancellationToken);
    account.Withdraw(amount);

    var transaction = new Transaction(account.Id, TransactionType.Withdrawal, amount, account.Balance);
    await _repository.UpdateWithTransactionAsync(account, transaction, cancellationToken);
    
    return new AccountResponseDto(account.Id, account.Balance);
  }

  private async Task<Account> GetAccountOrThrowAsync(Guid accountId, Guid userId, CancellationToken cancellationToken)
  {
    var account = await _repository.GetByIdAndUserIdAsync(accountId, userId, cancellationToken);
    if (account == null)
    {
      throw new AccountNotFoundException(accountId);
    }

    return account;
  }
}