using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SG.AccountService.Application.Interfaces;
using SG.AccountService.Application.DTOs;

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

  [HttpPost]
  [ProducesResponseType(typeof(AccountResponseDto), StatusCodes.Status201Created)]
  public async Task<ActionResult<AccountResponseDto>> CreateAccount(CancellationToken cancellationToken)
  {
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (!Guid.TryParse(userIdClaim, out var userId))
    {
      return Unauthorized("Token inválido.");
    }

    var account = await _accountService.CreateAccountAsync(userId, cancellationToken);

    return CreatedAtAction(nameof(GetBalance), new { id = account.AccountId }, account);
  }

  [HttpGet("{id:guid}/balance")]
  [ProducesResponseType(typeof(AccountResponseDto), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<AccountResponseDto>> GetBalance(Guid id, CancellationToken cancellationToken)
  {
    var response = await _accountService.GetBalanceAsync(id, cancellationToken);
    return Ok(response);
  }

  // TODO: Crear carpeta de DTO's en este proyecto
  public record TransactionRequest(decimal Amount);

  [HttpPost("{id:guid}/deposit")]
  [ProducesResponseType(typeof(AccountResponseDto), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<AccountResponseDto>> Deposit(Guid id, [FromBody] TransactionRequest request,
    CancellationToken cancellationToken)
  {
    var response = await _accountService.DepositAsync(id, request.Amount, cancellationToken);
    return Ok(response);
  }

  [HttpPost("{id:guid}/withdraw")]
  [ProducesResponseType(typeof(AccountResponseDto), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
  public async Task<ActionResult<AccountResponseDto>> Withdraw(Guid id, [FromBody] TransactionRequest request,
    CancellationToken cancellationToken)
  {
    var response = await _accountService.WithdrawAsync(id, request.Amount, cancellationToken);
    return Ok(response);
  }
}