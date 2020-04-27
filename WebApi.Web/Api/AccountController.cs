using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
  public class AccountController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    private const string UserNotFoundException = "User not found.";
    private const string InvalidDataException = "Invalid login or password";

    public AccountController(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      IJwtTokenService jwtTokenService,
      IMapper mapper,
      ILogger<AccountController> logger)
    {
      _userManager = userManager;
      _signInManager = signInManager; 
      _jwtTokenService = jwtTokenService;
      _mapper = mapper;
      _logger = logger;
    }

    [HttpPost(Name = "register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
      var user = _mapper.Map<ApplicationUser>(model);

      var result = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);

      if (result.Succeeded)
      {
        return Ok();
      }
      else
      {
        var errorMessage = string.Join(' ', result.Errors.Select(x => x.Description));

        throw new ArgumentException(errorMessage);
      }
    }

    [HttpPost(Name = "login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
      var user = await _userManager.FindByNameAsync(model.UserName).ConfigureAwait(false);

      if (user != null)
      {
        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true).ConfigureAwait(false);

        if (result.Succeeded == false)
        {
          throw new UnauthorizedAccessException(AccountController.InvalidDataException);
        }

        var claims = AccountController.FillClaims(user);

        var token = _jwtTokenService.GenerateJwtToken(claims, model.IsRememberMe);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        await _userManager.UpdateAsync(user).ConfigureAwait(false);

        return Ok(new {
          token = token,
          refreshToken = refreshToken
        });
      }

      throw new KeyNotFoundException(AccountController.UserNotFoundException);
    }

    private static IReadOnlyCollection<Claim> FillClaims(ApplicationUser user)
    {
      var claims = new List<Claim>
        {
          new Claim(TokenClaims.Email, user.Email),
          new Claim(TokenClaims.FirstName, user.FirstName),
          new Claim(TokenClaims.Id, user.Id),
          new Claim(TokenClaims.LastName, user.LastName),
          new Claim(TokenClaims.UserName, user.UserName)
        };

      return claims;
    }
  }
}