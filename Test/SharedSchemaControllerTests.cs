using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Schematics.API.Controllers;
using Schematics.API.DTOs.SharedSchema;
using Schematics.API.Service;
using System.Security.Claims;
using Xunit;
using Assert = Xunit.Assert;

public class SharedSchemaControllerTests
{
    private readonly Mock<ISharedSchemaService> _serviceMock;
    private readonly SharedSchemaController _controller;
    private const string UserId = "user-123";

    public SharedSchemaControllerTests()
    {
        _serviceMock = new Mock<ISharedSchemaService>();
        _controller = new SharedSchemaController(_serviceMock.Object);

        // Mock zalogowanego użytkownika
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, UserId)
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
    }

    [Fact]
    public async Task ShareSchema_WhenServiceReturnsFalse_ReturnsBadRequest()
    {
        var dto = new ShareSchemaRequestDto();

        _serviceMock
            .Setup(s => s.ShareSchemaAsync(UserId, dto))
            .ReturnsAsync(false);

        var result = await _controller.ShareSchema(dto);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task ShareSchema_WhenServiceReturnsTrue_ReturnsOk()
    {
        var dto = new ShareSchemaRequestDto();

        _serviceMock
            .Setup(s => s.ShareSchemaAsync(UserId, dto))
            .ReturnsAsync(true);

        var result = await _controller.ShareSchema(dto);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task GetSharedByMe_ReturnsOkWithResult()
    {
        var shared = new List<SharedSchemaDto>(); // dowolny DTO / kolekcja

        _serviceMock
            .Setup(s => s.GetSharedByOwnerAsync(UserId))
            .ReturnsAsync(shared);

        var result = await _controller.GetSharedByMe();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(shared, ok.Value);
    }

    [Fact]
    public async Task GetSharedWithMe_ReturnsOkWithResult()
    {
        var shared = new List<SharedSchemaDto>();

        _serviceMock
            .Setup(s => s.GetSharedWithUserAsync(UserId))
            .ReturnsAsync(shared);

        var result = await _controller.GetSharedWithMe();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(shared, ok.Value);
    }

    [Fact]
    public async Task RemoveShare_WhenServiceReturnsFalse_ReturnsNotFound()
    {
        _serviceMock
            .Setup(s => s.RemoveShareAsync(1, UserId))
            .ReturnsAsync(false);

        var result = await _controller.RemoveShare(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task RemoveShare_WhenServiceReturnsTrue_ReturnsOk()
    {
        _serviceMock
            .Setup(s => s.RemoveShareAsync(1, UserId))
            .ReturnsAsync(true);

        var result = await _controller.RemoveShare(1);

        Assert.IsType<OkResult>(result);
    }
}

