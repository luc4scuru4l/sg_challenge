using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SG.AuthService.Application.Interfaces;
using SG.AuthService.Application.DTOs;

namespace SG.AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class AuthController : ControllerBase
{
  private readonly IAuthService _authService;

  public AuthController(IAuthService authService)
  {
    _authService = authService;
  }

  /// <summary>
  /// Registra un nuevo usuario en el sistema.
  /// </summary>
  /// <remarks>
  /// Las credenciales a registrar deben cumplir con la política de seguridad:
  /// 
  /// **Constraseña:**
  /// 1. Mínimo 8 caracteres.
  /// 2. Máximo 32 caracteres.
  /// 3. Al menos un número.
  /// 
  /// **Nombre de usuario**
  /// 1. Menos de 50 caracteres
  /// 2. Más de 4 caracteres
  /// </remarks>
  [HttpPost("register")]
  [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
  public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
  {
    await _authService.RegisterAsync(request, ct);
    return StatusCode(StatusCodes.Status201Created);
  }

  /// <summary>
  /// Retorna un token de acceso.
  /// </summary>
  /// <remarks>
  /// Crea y retorna un token de acceso firmado para un usuario registrado.
  /// El token expira a los 20 minutos.
  /// </remarks>
  [HttpPost("login")]
  [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
  public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
  {
    var response = await _authService.LoginAsync(request, ct);
    return Ok(response);
  }
}