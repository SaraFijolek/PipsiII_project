using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Schematics.API.Controllers;
using Schematics.API.Data.Entities;
using Schematics.API.DTOs.Schemas;
using Schematics.API.Service;
using System.Security.Claims;
using Xunit;
using Assert = Xunit.Assert;

public class SchemaControllerTests
{
    private readonly Mock<ISchemaService> _schemaServiceMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly SchemaController _controller;

    public SchemaControllerTests()
    {
        _schemaServiceMock = new Mock<ISchemaService>();
        _userManagerMock = MockUserManager();

        _controller = new SchemaController(
            _schemaServiceMock.Object,
            _userManagerMock.Object
        );

        // Mock zalogowanego użytkownika
        var user = new User { Id = "user-1" };
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id)
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

        _userManagerMock
            .Setup(u => u.GetUserAsync(principal))
            .ReturnsAsync(user);
    }

    [Fact]
    public async Task AddSchema_ValidModel_ReturnsOk()
    {
        var dto = new AddSchemaDto
        {
            Name = "Schema 1"
        };

        var result = await _controller.AddSchema(dto);

        Assert.IsType<OkResult>(result);

        _schemaServiceMock.Verify(
            s => s.AddSchemaAsync(dto, "user-1"),
            Times.Once);
    }

    [Fact]
    public async Task GetAllSchemas_ReturnsOkWithSchemas()
    {
        var schemas = new List<SchemaDto>
        {
            new SchemaDto(),
            new SchemaDto()
        };

        _schemaServiceMock
            .Setup(s => s.GetAllSchemasAsync())
            .ReturnsAsync(schemas);

        var result = await _controller.GetAllSchemas();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<List<SchemaDto>>(ok.Value);

        Assert.Equal(2, returned.Count);
    }

    private static Mock<UserManager<User>> MockUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(
            store.Object,
            null, null, null, null, null, null, null, null);
    }
}
