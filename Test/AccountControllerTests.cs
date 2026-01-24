using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Schematics.API.Controllers;
using Schematics.API.Data.Entities;
using Schematics.API.DTOs.Account;
using Schematics.API.DTOs.Books;
using Schematics.API.Service;
using Schematics.API.Service.Infrastructure;
using Xunit;
using Assert = Xunit.Assert;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

public class AccountControllerTests
{
    private readonly Mock<IBookService> _bookServiceMock = new();
    private readonly Mock<IJwtTokenService> _tokenServiceMock = new();
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<SignInManager<User>> _signInManagerMock;

    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        _userManagerMock = MockUserManager();
        _signInManagerMock = MockSignInManager();

        _controller = new AccountController(
            _bookServiceMock.Object,
            _tokenServiceMock.Object,
            _userManagerMock.Object,
            _signInManagerMock.Object
        );
    }

    [Fact]
    public async Task LoginAsync_UserNotFound_ReturnsBadRequest()
    {
        _userManagerMock
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null);

        var result = await _controller.LoginAsync(new LoginDto
        {
            Email = "test@test.com",
            Password = "123"
        });

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ReturnsBadRequest()
    {
        var user = new User { Id = "1", Email = "test@test.com" };

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(x => x.PasswordSignInAsync(user, "wrong", true, true))
            .ReturnsAsync(SignInResult.Failed);

        var result = await _controller.LoginAsync(new LoginDto
        {
            Email = user.Email,
            Password = "wrong"
        });

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        var user = new User { Id = "1", Email = "test@test.com" };

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(x => x.PasswordSignInAsync(user, "pass", true, true))
            .ReturnsAsync(SignInResult.Success);

        _tokenServiceMock
            .Setup(x => x.CreateToken(user.Id))
            .Returns("jwt-token");

        var result = await _controller.LoginAsync(new LoginDto
        {
            Email = user.Email,
            Password = "pass"
        });

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task RegisterAsync_PasswordsDoNotMatch_ReturnsBadRequest()
    {
        var result = await _controller.RegisterAsync(new RegisterDto
        {
            Email = "test@test.com",
            Username = "user",
            Password = "123",
            ConfirmPassword = "456"
        });

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task RegisterAsync_ValidData_ReturnsOk()
    {
        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _controller.RegisterAsync(new RegisterDto
        {
            Email = "test@test.com",
            Username = "user",
            Password = "123",
            ConfirmPassword = "123"
        });

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task GetAllUsers_BooksExist_ReturnsOk()
    {
        _bookServiceMock
            .Setup(x => x.GetAllBooksAsync())
            .ReturnsAsync(new List<BookDto> { new BookDto() });

        var result = await _controller.GetAllUsers();

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAllUsers_NoBooks_ReturnsNotFound()
    {
        _bookServiceMock
            .Setup(x => x.GetAllBooksAsync())
            .ReturnsAsync(new List<BookDto>());

        var result = await _controller.GetAllUsers();

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task AddUser_ValidBook_ReturnsOk()
    {
        var result = await _controller.AddUser(new AddBookDto());

        Assert.IsType<OkResult>(result);
        _bookServiceMock.Verify(x => x.AddBookAsync(It.IsAny<AddBookDto>()), Times.Once);
    }
    private static Mock<UserManager<User>> MockUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(
            store.Object, null, null, null, null, null, null, null, null);
    }

    private static Mock<SignInManager<User>> MockSignInManager()
    {
        return new Mock<SignInManager<User>>(
            MockUserManager().Object,
            new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<User>>().Object,
            null, null, null, null);
    }
}
