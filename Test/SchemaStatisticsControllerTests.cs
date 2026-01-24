using Microsoft.AspNetCore.Mvc;
using Moq;
using Schematics.API.Controllers;
using Schematics.API.DTOs.SchemaStatistics;
using Schematics.API.Service;
using Xunit;
using Assert = Xunit.Assert;

public class SchemaStatisticsControllerTests
{
    private readonly Mock<ISchemaStatisticsService> _serviceMock;
    private readonly SchemaStatisticsController _controller;

    public SchemaStatisticsControllerTests()
    {
        _serviceMock = new Mock<ISchemaStatisticsService>();
        _controller = new SchemaStatisticsController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetStatisticsBySchemaId_NotFound_ReturnsNotFound()
    {
        _serviceMock
            .Setup(s => s.GetStatisticsBySchemaIdAsync(1))
            .ReturnsAsync((SchemaStatisticsDto)null);

        var result = await _controller.GetStatisticsBySchemaId(1);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetStatisticsBySchemaId_Exists_ReturnsOk()
    {
        var dto = new SchemaStatisticsDto();

        _serviceMock
            .Setup(s => s.GetStatisticsBySchemaIdAsync(1))
            .ReturnsAsync(dto);

        var result = await _controller.GetStatisticsBySchemaId(1);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task GetAllStatistics_ReturnsOkWithList()
    {
        var list = new List<SchemaStatisticsDto>
        {
            new SchemaStatisticsDto(),
            new SchemaStatisticsDto()
        };

        _serviceMock
            .Setup(s => s.GetAllStatisticsAsync())
            .ReturnsAsync(list);

        var result = await _controller.GetAllStatistics();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<List<SchemaStatisticsDto>>(ok.Value);
        Assert.Equal(2, returned.Count);
    }

    [Fact]
    public async Task UpdateStatistics_ValidCall_ReturnsOk()
    {
        var dto = new UpdateStatisticsDto();

        var result = await _controller.UpdateStatistics(1, dto);

        Assert.IsType<OkResult>(result);

        _serviceMock.Verify(
            s => s.UpdateStatisticsAsync(1, dto),
            Times.Once);
    }

    [Fact]
    public async Task RecalculateStatistics_ValidCall_ReturnsOk()
    {
        var result = await _controller.RecalculateStatistics(1);

        Assert.IsType<OkResult>(result);

        _serviceMock.Verify(
            s => s.RecalculateStatisticsAsync(1),
            Times.Once);
    }
}

