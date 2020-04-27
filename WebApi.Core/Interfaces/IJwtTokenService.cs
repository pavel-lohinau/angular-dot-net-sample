using System.Collections.Generic;
using System.Security.Claims;

namespace WebApi.Core.Interfaces
{
  public interface IJwtTokenService
  {
    string GenerateJwtToken(IEnumerable<Claim> claims, bool? rememberUser);

    string GenerateRefreshToken();

    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
  }
}
