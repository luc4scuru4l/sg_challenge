using SG.AccountService.Application.DTOs;

namespace SG.AccountService.Application.Interfaces;

public interface IAccountService
{
    // Crea una nueva cuenta para un determinado usuario
    Task<AccountResponseDto> CreateAccountAsync(Guid userId, CancellationToken cancellationToken = default);
    
    // Retorna el balance de una cuenta
    Task<decimal> GetBalanceAsync(Guid accountId, CancellationToken cancellationToken = default);
    
    // Realiza un depósito en una cuenta
    Task DepositAsync(Guid accountId, decimal amount, CancellationToken cancellationToken = default);
    
    // Realiza un retiro en una cuenta
    Task WithdrawAsync(Guid accountId, decimal amount, CancellationToken cancellationToken = default);
}