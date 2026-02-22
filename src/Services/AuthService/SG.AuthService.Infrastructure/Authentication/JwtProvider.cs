using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SG.AuthService.Application.Interfaces;
using SG.AuthService.Domain.Entities;

namespace SG.AuthService.Infrastructure.Authentication;

public class JwtProvider : IJwtProvider
{
  private readonly JwtSettings _jwtSettings;

  public JwtProvider(IOptions<JwtSettings> jwtOptions)
  {
    _jwtSettings = jwtOptions.Value;
  }

  public string Generate(User user)
  {
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
      new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
      new Claim(JwtRegisteredClaimNames.Name, user.UserName),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var token = new JwtSecurityToken(
      issuer: _jwtSettings.Issuer,
      audience: _jwtSettings.Audience,
      claims: claims,
      expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
      signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}