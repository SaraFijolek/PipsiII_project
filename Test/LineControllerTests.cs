using Microsoft.AspNetCore.Mvc;
using Moq;
using Schematics.API.Controllers;
using Schematics.API.DTOs.Lines;
using Schematics.API.Service;
using Xunit;
using Assert = Xunit.Assert;

public class LineControllerTests
{
    private readonly Mock<ILineService> _lineServiceMock;
    private readonly LineController _controller;

    public LineControllerTests()
    {
        _lineServiceMock = new Mock<ILineService>();
        _controller = new LineController(_lineServiceMock.Object);
    }

    [Fact]
    public async Task AddLine_ValidModel_ReturnsOk()
    {
        var dto = new LineDto();

        var result = await _controller.AddLine(dto);

        Assert.IsType<OkResult>(result);

        _lineServiceMock.Verify(
            s => s.AddLineAsync(dto),
            Times.Once);
    }

    [Fact]
    public async Task GetAllLines_ReturnsOkWithList()
    {
        var lines = new List<LineDto>
        {
            new LineDto(),
            new LineDto()
        };

        _lineServiceMock
            .Setup(s => s.GetAllLinesAsync())
            .ReturnsAsync(lines);

        var result = await _controller.GetAllLines();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<List<LineDto>>(ok.Value);
        Assert.Equal(2, returned.Count);
    }

    [Fact]
    public async Task GetLinesBySchemaId_ReturnsOkWithLines()
    {
        var lines = new List<LineDto>
        {
            new LineDto()
        };

        _lineServiceMock
            .Setup(s => s.GetLinesBySchemaIdAsync(10))
            .ReturnsAsync(lines);

        var result = await _controller.GetLinesBySchemaId(10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<List<LineDto>>(ok.Value);
        Assert.Single(returned);
    }
}
