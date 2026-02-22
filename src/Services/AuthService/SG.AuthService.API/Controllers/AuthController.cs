using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SG.AuthService.Application.Interfaces;
using SG.AuthService.Application.DTOs;

namespace SG.AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
  private readonly IAuthService _authService;

  public AuthController(IAuthService authService)
  {
    _authService = authService;
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
  {
    await _authService.RegisterAsync(request, ct);
    return StatusCode(StatusCodes.Status201Created, new { message = "Usuario registrado exitosamente." });
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
  {
    var response = await _authService.LoginAsync(request, ct);
    return Ok(response);
  }
}