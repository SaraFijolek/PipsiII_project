using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Schematics.API.Controllers;
using Schematics.API.Data.Entities;
using Schematics.API.Service.Infrastructure;
using System.Security.Claims;
using Xunit;
using Assert = Xunit.Assert;

public class ExternalAuthControllerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly Mock<IJwtTokenService> _jwtServiceMock;

    private readonly ExternalAuthController _controller;

    public ExternalAuthControllerTests()
    {
        _userManagerMock = MockUserManager();
        _signInManagerMock = MockSignInManager(_userManagerMock.Object);
        _jwtServiceMock = new Mock<IJwtTokenService>();

        _controller = new ExternalAuthController(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _jwtServiceMock.Object
        );

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public void GoogleLogin_ReturnsChallengeResult()
    {
        var result = _controller.GoogleLogin(); //null w GoogleLogin()

        var challenge = Assert.IsType<ChallengeResult>(result);
        Assert.Contains("Google", challenge.AuthenticationSchemes[0]);
    }

    [Fact]
    public void FacebookLogin_ReturnsChallengeResult()
    {
        var result = _controller.FacebookLogin(); //null w FacebookLogin()

        var challenge = Assert.IsType<ChallengeResult>(result);
        Assert.Contains("Facebook", challenge.AuthenticationSchemes[0]);
    }

    [Fact]
    public async Task ExternalLoginCallback_NoExternalInfo_ReturnsBadRequest()
    {
        _signInManagerMock
            .Setup(x => x.GetExternalLoginInfoAsync(It.IsAny<string>()))
            .ReturnsAsync((ExternalLoginInfo)null);

        var result = await _controller.ExternalLoginCallback();

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Błąd logowania zewnętrznego", badRequest.Value);
    }

    [Fact]
    public async Task ExternalLoginCallback_NoEmailClaim_ReturnsBadRequest()
    {
        var info = CreateExternalLoginInfo(null);

        _signInManagerMock
            .Setup(x => x.GetExternalLoginInfoAsync(It.IsAny<string>()))
            .ReturnsAsync(info);

        var result = await _controller.ExternalLoginCallback();

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Brak emaila od providera", badRequest.Value);
    }

    [Fact]
    public async Task ExternalLoginCallback_NewUser_CreatesUserAndReturnsToken()
    {
        var email = "test@test.com";
        var user = new User { Id = "1", Email = email };

        var info = CreateExternalLoginInfo(email);

        _signInManagerMock
            .Setup(x => x.GetExternalLoginInfoAsync(It.IsAny<string>()))
            .ReturnsAsync(info);

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(email))
            .ReturnsAsync((User)null);

        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success);

        _jwtServiceMock
            .Setup(x => x.CreateToken(It.IsAny<string>()))
            .Returns("jwt-token");

        var result = await _controller.ExternalLoginCallback();

        Assert.IsType<OkObjectResult>(result);

        _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
        _userManagerMock.Verify(x => x.AddLoginAsync(It.IsAny<User>(), info), Times.Once);
    }

    [Fact]
    public async Task ExternalLoginCallback_ExistingUserWithoutLogin_AddsLogin()
    {
        var email = "test@test.com";
        var user = new User { Id = "1", Email = email };

        var info = CreateExternalLoginInfo(email);

        _signInManagerMock
            .Setup(x => x.GetExternalLoginInfoAsync(It.IsAny<string>()))
            .ReturnsAsync(info);

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(x => x.GetLoginsAsync(user))
            .ReturnsAsync(new List<UserLoginInfo>());

        _jwtServiceMock
            .Setup(x => x.CreateToken(user.Id))
            .Returns("jwt-token");

        var result = await _controller.ExternalLoginCallback();

        Assert.IsType<OkObjectResult>(result);

        _userManagerMock.Verify(x => x.AddLoginAsync(user, info), Times.Once);
    }

    private static ExternalLoginInfo CreateExternalLoginInfo(string? email)
    {
        var claims = new List<Claim>();

        if (email != null)
        {
            claims.Add(new Claim(ClaimTypes.Email, email));
        }

        var identity = new ClaimsIdentity(claims, "Google");
        var principal = new ClaimsPrincipal(identity);

        return new ExternalLoginInfo(
            principal,
            "Google",
            "provider-key",
            "Google");
    }

    private static Mock<UserManager<User>> MockUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(
            store.Object, null, null, null, null, null, null, null, null);
    }

    private static Mock<SignInManager<User>> MockSignInManager(UserManager<User> userManager)
    {
        return new Mock<SignInManager<User>>(
            userManager,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<User>>().Object,
            null, null, null, null);
    }
}
