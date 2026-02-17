using System.Security.Claims; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SG.AccountService.Application.Interfaces;
using SG.AccountService.Application.DTOs;

namespace SG.AccountService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountController : ControllerBase
{
  private readonly IAccountService _accountService;

  public AccountController(IAccountService accountService)
  {
    _accountService = accountService;
  }

  [HttpPost]
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
  public async Task<ActionResult<AccountResponseDto>> GetBalance(Guid id, CancellationToken cancellationToken)
  {
    try
    {
      var balance = await _accountService.GetBalanceAsync(id, cancellationToken);
      return Ok(new AccountResponseDto(id, balance));
    }
    catch (KeyNotFoundException ex)
    {
      return NotFound(ex.Message);
    }
  }
}