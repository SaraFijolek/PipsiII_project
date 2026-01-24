using Microsoft.AspNetCore.Mvc;
using Moq;
using Schematics.API.Controllers;
using Schematics.API.DTOs.Station;
using Schematics.API.Service;
using Xunit;
using Assert = Xunit.Assert;

public class StationControllerTests
{
    private readonly Mock<IStationService> _stationServiceMock;
    private readonly StationController _controller;

    public StationControllerTests()
    {
        _stationServiceMock = new Mock<IStationService>();
        _controller = new StationController(_stationServiceMock.Object);
    }

    [Fact]
    public async Task AddStation_ValidModel_ReturnsOk()
    {
        var dto = new AddStation();

        var result = await _controller.AddStation(dto);

        Assert.IsType<OkResult>(result);

        _stationServiceMock.Verify(
            s => s.AddStationAsync(dto),
            Times.Once);
    }

    [Fact]
    public async Task GetStationsBySchema_ReturnsOkWithStations()
    {
        var stations = new List<StationDto>
        {
            new StationDto(),
            new StationDto()
        };

        _stationServiceMock
            .Setup(s => s.GetStationsBySchemaIdAsync(10))
            .ReturnsAsync(stations);

        var result = await _controller.GetStationsBySchema(10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<List<StationDto>>(ok.Value);
        Assert.Equal(2, returned.Count);
    }

    [Fact]
    public async Task UpdateStation_ValidCall_ReturnsOk()
    {
        var dto = new EditStation();

        var result = await _controller.UpdateStation(1, dto);

        Assert.IsType<OkResult>(result);

        _stationServiceMock.Verify(
            s => s.UpdateStationAsync(1, dto),
            Times.Once);
    }

    [Fact]
    public async Task DeleteStation_ValidCall_ReturnsOk()
    {
        var result = await _controller.DeleteStation(1);

        Assert.IsType<OkResult>(result);

        _stationServiceMock.Verify(
            s => s.DeleteStationAsync(1),
            Times.Once);
    }
}
