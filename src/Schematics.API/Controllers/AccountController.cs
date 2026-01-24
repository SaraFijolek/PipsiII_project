using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schematics.API.Data;
using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using Schematics.API.DTOs.Account;
using Schematics.API.DTOs.Books;
using Schematics.API.Service;
using Schematics.API.Service.Infrastructure;
using System.Data;
using System.Security.Claims;
using System.Text.Json;


namespace Schematics.API.Controllers;

[Authorize]
[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private IBookService _bookService;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtTokenService _tokenService;
    
    public AccountController(IBookService bookService, IJwtTokenService tokenService,
        UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _bookService = bookService;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult> LoginAsync([FromBody] LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            return BadRequest();
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);

        if (result.Succeeded)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.CreateToken(user.Id, roles);
            return Ok(new { access_token = token });
        }
        else
        {
            return BadRequest();
        }
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync([FromBody] RegisterDto model)
    {
        var user = new User
        {
            Email = model.Email,
            UserName = model.Username,
            Firstname = model.FirstName,
            Lastname = model.LastName
        };

        if (model.Password != model.ConfirmPassword)
        {
            return BadRequest();
        }

        await _userManager.CreateAsync(user, model.Password);
        return Ok();
    }




    

    [HttpGet("my-info")]
    [Authorize]
    public async Task<IActionResult> GetMyInfo()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);
        var claimsFromToken = User.Claims.Select(c => new { c.Type, c.Value });

        return Ok(new
        {
            userId = user.Id,
            email = user.Email,
            rolesInDatabase = roles,
            claimsInToken = claimsFromToken,
            hasAdminRole = User.IsInRole("Admin")
        });
    }



    [HttpGet("books")]
    public async Task<ActionResult<IList<BookDto>>> GetAllUsers()
    {
        var books = await _bookService.GetAllBooksAsync();

        if (books.Any())
        {
            return Ok(books);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpPost("books")]
    public async Task<ActionResult> AddUser([FromBody] AddBookDto book)
    {
        await _bookService.AddBookAsync(book);

        return Ok();
    }
}
