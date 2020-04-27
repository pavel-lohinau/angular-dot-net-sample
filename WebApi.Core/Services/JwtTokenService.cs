using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using WebApi.Core.Entities;
using WebApi.Core.Interfaces;

namespace WebApi.Core.Services
{
  public class JwtTokenService : IJwtTokenService
  {
    private readonly TokenSettings _tokenSettings;
    private const string InvalidTokenException = "Invalid token";

    public JwtTokenService(TokenSettings tokenSettings)
    {
      _tokenSettings = tokenSettings;
    }

    public string GenerateJwtToken(IEnumerable<Claim> claims, bool? rememberUser)
    {
      var expiresDate = DateTime.UtcNow.AddMinutes(_tokenSettings.AccessExpiration);
      if (rememberUser.HasValue && rememberUser.Value)
      {
        expiresDate = DateTime.UtcNow.AddDays(_tokenSettings.AccessExpiration);
      }

      var token = new JwtSecurityToken
      (
          issuer: _tokenSettings.Issuer,
          audience: _tokenSettings.Audience,
          claims: claims,
          expires: expiresDate,
          notBefore: DateTime.UtcNow,
          signingCredentials: new SigningCredentials(_tokenSettings.SecurityKey,
                  SecurityAlgorithms.HmacSha256)
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
      var randomNumber = new byte[32];
      using var rng = RandomNumberGenerator.Create();
      rng.GetBytes(randomNumber);
      return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
      var tokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = _tokenSettings.SecurityKey,
        ValidateIssuer = true,
        ValidIssuer = _tokenSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = _tokenSettings.Audience,
        ValidateLifetime = false
      };

      var principal = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
      var jwtSecurityToken = securityToken as JwtSecurityToken;

      if (this.IsJwtSecurityTokenInvalid(jwtSecurityToken))
        throw new SecurityTokenException(JwtTokenService.InvalidTokenException);

      return principal;
    }

    private bool IsJwtSecurityTokenInvalid(JwtSecurityToken jwtSecurityToken)
    {
      return jwtSecurityToken == null || jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase) == false;
    }
  }
}