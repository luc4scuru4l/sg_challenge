using System.Security.Claims;
using SG.AccountService.Application.Exceptions;

namespace SG.AccountService.API.Extensions;

public static class ClaimsPrincipalExtensions
{
  public static Guid GetUserId(this ClaimsPrincipal user)
  {
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (!Guid.TryParse(userIdClaim, out var userId))
    {
      throw new UnauthorizedException();
    }

    return userId;
  }
}