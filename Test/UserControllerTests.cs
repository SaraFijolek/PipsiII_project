using Microsoft.AspNetCore.Mvc;
using Moq;
using Schematics.API.Controllers;
using Schematics.API.DTOs.Schemas;
using Schematics.API.DTOs.Users;
using Schematics.API.Service;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ISchemaService> _schemaServiceMock;
    private readonly Mock<ILogService> _logServiceMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _schemaServiceMock = new Mock<ISchemaService>();
        _logServiceMock = new Mock<ILogService>();

        _controller = new UserController(
            _userServiceMock.Object,
            _schemaServiceMock.Object,
            _logServiceMock.Object
        );
    }

    [Fact]
    public async Task GetAllUsers_ReturnsOkWithList()
    {
        var users = new List<UserDto> { new UserDto(), new UserDto() };
        _userServiceMock.Setup(s => s.GetAllUsers()).ReturnsAsync(users);

        var result = await _controller.GetAllUsers();

        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<List<UserDto>>(ok.Value);
        Assert.Equal(2, returned.Count);
    }

    [Fact]
    public async Task GetUserById_NotFound_ReturnsNotFound()
    {
        _userServiceMock.Setup(s => s.GetUserById("1")).ReturnsAsync((UserDto)null);

        var result = await _controller.GetUserById("1");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetUserById_Found_ReturnsOk()
    {
        var user = new UserDto();
        _userServiceMock.Setup(s => s.GetUserById("1")).ReturnsAsync(user);

        var result = await _controller.GetUserById("1");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(user, ok.Value);
    }

    [Fact]
    public async Task UpdateUser_Failure_ReturnsBadRequest()
    {
        var dto = new EditUserDto();
        _userServiceMock.Setup(s => s.UpdateUser(dto)).ReturnsAsync(false);

        var result = await _controller.UpdateUser(dto);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task UpdateUser_Success_ReturnsOk()
    {
        var dto = new EditUserDto();
        _userServiceMock.Setup(s => s.UpdateUser(dto)).ReturnsAsync(true);

        var result = await _controller.UpdateUser(dto);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task LockUser_NotFound_ReturnsNotFound()
    {
        _userServiceMock.Setup(s => s.LockUser("1")).ReturnsAsync(false);

        var result = await _controller.LockUser("1");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task LockUser_Success_ReturnsOk()
    {
        _userServiceMock.Setup(s => s.LockUser("1")).ReturnsAsync(true);

        var result = await _controller.LockUser("1");

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UnlockUser_NotFound_ReturnsNotFound()
    {
        _userServiceMock.Setup(s => s.UnlockUser("1")).ReturnsAsync(false);

        var result = await _controller.UnlockUser("1");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UnlockUser_Success_ReturnsOk()
    {
        _userServiceMock.Setup(s => s.UnlockUser("1")).ReturnsAsync(true);

        var result = await _controller.UnlockUser("1");

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task AddRole_Failure_ReturnsBadRequest()
    {
        _userServiceMock.Setup(s => s.AddUserToRole("1", "Admin")).ReturnsAsync(false);

        var result = await _controller.AddRole("1", "Admin");

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task AddRole_Success_ReturnsOk()
    {
        _userServiceMock.Setup(s => s.AddUserToRole("1", "Admin")).ReturnsAsync(true);

        var result = await _controller.AddRole("1", "Admin");

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task RemoveRole_Failure_ReturnsBadRequest()
    {
        _userServiceMock.Setup(s => s.RemoveUserFromRole("1", "Admin")).ReturnsAsync(false);

        var result = await _controller.RemoveRole("1", "Admin");

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task RemoveRole_Success_ReturnsOk()
    {
        _userServiceMock.Setup(s => s.RemoveUserFromRole("1", "Admin")).ReturnsAsync(true);

        var result = await _controller.RemoveRole("1", "Admin");

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task GetAllSchemas_ReturnsOkWithList()
    {
        var schemas = new List<SchemaDto> { new SchemaDto() };
        _schemaServiceMock.Setup(s => s.GetAllSchemasAsync()).ReturnsAsync(schemas);

        var result = await _controller.GetAllSchemas();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(schemas, ok.Value);
    }

    [Fact]
    public async Task GetSchemaById_NotFound_ReturnsNotFound()
    {
        _schemaServiceMock.Setup(s => s.GetSchemaByIdAsync(1)).ReturnsAsync((SchemaDto)null);

        var result = await _controller.GetSchemaById(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetSchemaById_Found_ReturnsOk()
    {
        var schema = new SchemaDto();
        _schemaServiceMock.Setup(s => s.GetSchemaByIdAsync(1)).ReturnsAsync(schema);

        var result = await _controller.GetSchemaById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(schema, ok.Value);
    }

    [Fact]
    public async Task UpdateSchema_NotFound_ReturnsNotFound()
    {
        var dto = new AddSchemaDto();
        _schemaServiceMock.Setup(s => s.UpdateSchemaAsync(1, dto)).ReturnsAsync(false);

        var result = await _controller.UpdateSchema(1, dto);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateSchema_Success_ReturnsOk()
    {
        var dto = new AddSchemaDto();
        _schemaServiceMock.Setup(s => s.UpdateSchemaAsync(1, dto)).ReturnsAsync(true);

        var result = await _controller.UpdateSchema(1, dto);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteSchema_NotFound_ReturnsNotFound()
    {
        _schemaServiceMock.Setup(s => s.DeleteSchemaAsync(1)).ReturnsAsync(false);

        var result = await _controller.DeleteSchema(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteSchema_Success_ReturnsOk()
    {
        _schemaServiceMock.Setup(s => s.DeleteSchemaAsync(1)).ReturnsAsync(true);

        var result = await _controller.DeleteSchema(1);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task GetLogs_ReturnsOkWithContent()
    {
        var logs = "line1\nline2";
        List<string> strings = new List<string>() { logs };
        _logServiceMock.Setup(s => s.ReadLastLinesAsync(200)).ReturnsAsync(strings);

        var result = await _controller.GetLogs();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(strings, ok.Value);
    }
}
