using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SG.AccountService.Application.Interfaces;
using SG.AccountService.Application.DTOs;
using  SG.AccountService.API.Extensions;

namespace SG.AccountService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class AccountsController : ControllerBase
{
  private readonly IAccountService _accountService;

  public AccountsController(IAccountService accountService)
  {
    _accountService = accountService;
  }

  /// <summary>
  /// Crea una cuenta con balance 0.
  /// </summary>
  [HttpPost]
  [ProducesResponseType(typeof(AccountResponseDto), StatusCodes.Status201Created)]
  public async Task<ActionResult<AccountResponseDto>> CreateAccount(CancellationToken cancellationToken)
  {
    var userId = User.GetUserId();
    var account = await _accountService.CreateAccountAsync(userId, cancellationToken);

    return CreatedAtAction(nameof(GetBalance), new { id = account.AccountId }, account);
  }

  /// <summary>
  /// Consulta el balance de una cuenta existente.
  /// </summary>
  [HttpGet("{id:guid}/balance")]
  [ProducesResponseType(typeof(AccountResponseDto), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<AccountResponseDto>> GetBalance(Guid id, CancellationToken cancellationToken)
  {
    var userId = User.GetUserId();
    var response = await _accountService.GetBalanceAsync(id, userId, cancellationToken);
    return Ok(response);
  }

  // TODO: Crear carpeta de DTO's en este proyecto
  public record TransactionRequest(decimal Amount);

  /// <summary>
  /// Realiza un deposito en una cuenta.
  /// </summary>
  /// <remarks>
  /// Aumenta el balance de una cuenta existente.
  /// </remarks>
  [HttpPost("{id:guid}/deposit")]
  [ProducesResponseType(typeof(AccountResponseDto), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<AccountResponseDto>> Deposit(Guid id, [FromBody] TransactionRequest request,
    CancellationToken cancellationToken)
  {
    var userId = User.GetUserId();
    var response = await _accountService.DepositAsync(id, userId, request.Amount, cancellationToken);
    return Ok(response);
  }

  /// <summary>
  /// Realiza un retiro en una cuenta existente.
  /// </summary>
  /// <remarks>
  /// Disminuye el balance de una cuenta. El monto a retirar no puede exceder el saldo actual de la cuenta.
  /// </remarks>
  [HttpPost("{id:guid}/withdraw")]
  [ProducesResponseType(typeof(AccountResponseDto), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
  public async Task<ActionResult<AccountResponseDto>> Withdraw(Guid id, [FromBody] TransactionRequest request,
    CancellationToken cancellationToken)
  {
    var userId = User.GetUserId();
    var response = await _accountService.WithdrawAsync(id, userId, request.Amount, cancellationToken);
    return Ok(response);
  }
}