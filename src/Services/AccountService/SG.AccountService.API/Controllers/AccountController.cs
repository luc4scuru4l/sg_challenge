using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SG.AccountService.Application.Interfaces;
using SG.AccountService.Application.DTOs;

namespace SG.AccountService.API.Controllers;

// TODO: Estaría bueno tener excepciones personalizadas e implementar un middleware global para reducir un poco los try catch's 

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class AccountController : ControllerBase
{
  private readonly IAccountService _accountService;

  public AccountController(IAccountService accountService)
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
    var balance = await _accountService.GetBalanceAsync(id, cancellationToken);
    return Ok(new AccountResponseDto(id, balance));
  }

  // TODO: Crear carpeta de DTO's en este proyecto
  public record TransactionRequest(decimal Amount);

  [HttpPost("{id:guid}/deposit")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> Deposit(Guid id, [FromBody] TransactionRequest request,
    CancellationToken cancellationToken)
  {
    await _accountService.DepositAsync(id, request.Amount, cancellationToken);
    return Ok();
  }

  [HttpPost("{id:guid}/withdraw")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
  public async Task<IActionResult> Withdraw(Guid id, [FromBody] TransactionRequest request,
    CancellationToken cancellationToken)
  {
    await _accountService.WithdrawAsync(id, request.Amount, cancellationToken);
    return Ok();
  }
}