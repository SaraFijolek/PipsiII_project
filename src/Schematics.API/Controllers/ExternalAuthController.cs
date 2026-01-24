using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Schematics.API.Data.Entities;
using Schematics.API.Service.Infrastructure;
using System.Security.Claims;

namespace Schematics.API.Controllers;

[ApiController]
[Route("external-auth")]
public class ExternalAuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtTokenService _jwtService;

    public ExternalAuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtTokenService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    
    [HttpGet("google")]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "ExternalAuth");
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
        return Challenge(properties, "Google");
    }

   
    [HttpGet("facebook")]
    public IActionResult FacebookLogin()
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "ExternalAuth");
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
        return Challenge(properties, "Facebook");
    }

    
    [AllowAnonymous]
    [HttpGet("callback")]
    public async Task<IActionResult> ExternalLoginCallback()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return BadRequest("Błąd logowania zewnętrznego");

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);

        if (email == null)
            return BadRequest("Brak emaila od providera");

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new User
            {
                UserName = email,
                Email = email
            };

            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user, info);
        }
        else
        {
            var logins = await _userManager.GetLoginsAsync(user);
            if (!logins.Any(l =>
                l.LoginProvider == info.LoginProvider &&
                l.ProviderKey == info.ProviderKey))
            {
                await _userManager.AddLoginAsync(user, info);
            }
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.CreateToken(user.Id,roles);

        return Ok(new { access_token = token });
    }
}
