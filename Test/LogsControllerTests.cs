using Microsoft.AspNetCore.Mvc;
using Moq;
using Schematics.API.Controllers;
using Schematics.API.Service;
using Xunit;
using Assert = Xunit.Assert;

public class LogsControllerTests
{
    private readonly Mock<ILogService> _logServiceMock;
    private readonly LogsController _controller;

    public LogsControllerTests()
    {
        _logServiceMock = new Mock<ILogService>();
        _controller = new LogsController(_logServiceMock.Object);
    }

    [Fact]
    public async Task Get_DefaultLines_ReturnsOkWithContent()
    {
        var logContent = new List<string>() { "log1\nlog2\nlog3" };

        _logServiceMock
            .Setup(s => s.ReadLastLinesAsync(200))
            .ReturnsAsync(logContent);

        var result = await _controller.Get();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(logContent, ok.Value);
    }

    [Fact]
    public async Task Get_CustomLines_ReturnsOkWithContent()
    {
        var logContent = new List<string>() { "custom logs" };

        _logServiceMock
            .Setup(s => s.ReadLastLinesAsync(50))
            .ReturnsAsync(logContent);

        var result = await _controller.Get(50);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(logContent, ok.Value);
    }

    [Fact]
    public async Task Get_CallsServiceOnce()
    {
        await _controller.Get(10);

        _logServiceMock.Verify(
            s => s.ReadLastLinesAsync(10),
            Times.Once);
    }
}

