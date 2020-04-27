using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Core.Interfaces;
using WebApi.Infrastructure.Identity;
using WebApi.Web.Helpers;
using WebApi.Web.Models;

namespace WebApi.Web.Api
{
  [AllowAnonymous]
  [ApiController]
  [Route("api/[controller]/[action]")]
  public class TokenController : ControllerBase
  {
    private readonly IJwtTokenService _jwtTokenService;
    private readonly UserManager<ApplicationUser> _userManager;

    public TokenController(
      IJwtTokenService jwtTokenService,
      UserManager<ApplicationUser> userManager)
    {
      _jwtTokenService = jwtTokenService;
      _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Refresh(RefreshTokenModel requestModel)
    {
      var principal = _jwtTokenService.GetPrincipalFromExpiredToken(requestModel.Token);
      var username = principal.Claims.SingleOrDefault(x => x.Type == TokenClaims.UserName).Value;

      var user = await _userManager.FindByNameAsync(username).ConfigureAwait(false);

      if (user == null || user.RefreshToken != requestModel.RefreshToken)
      {
        return BadRequest();
      }

      var newJwtToken = _jwtTokenService.GenerateJwtToken(principal.Claims, null);
      var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

      user.RefreshToken = newRefreshToken;
      await _userManager.UpdateAsync(user).ConfigureAwait(false);

      return Ok(new
      {
        token = newJwtToken,
        refreshToken = newRefreshToken
      });
    }

    [HttpPost]
    public async Task<IActionResult> Revoke(RefreshTokenModel requestModel)
    {
      var principal = _jwtTokenService.GetPrincipalFromExpiredToken(requestModel.Token);
      var username = principal.Claims.SingleOrDefault(x => x.Type == TokenClaims.UserName).Value;

      var user = await _userManager.FindByNameAsync(username).ConfigureAwait(false);
      if (user == null)
      {
        return BadRequest();
      }

      user.RefreshToken = null;

      await _userManager.UpdateAsync(user).ConfigureAwait(false);

      return NoContent();
    }
  }
}
