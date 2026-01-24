using Moq;
using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using Schematics.API.DTOs.Station;
using Schematics.API.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

public class StationServiceTests
{
    private Mock<IStationRepository> _mockRepo;
    private Mock<IStationLineRepository> _mockStationLineRepo;
    private StationService _service;

    public StationServiceTests()
    {
        _mockRepo = new Mock<IStationRepository>();
        _mockStationLineRepo = new Mock<IStationLineRepository>();
        _service = new StationService(_mockRepo.Object, _mockStationLineRepo.Object);
    }

    [Fact]
    public async Task AddStationAsync_AddsStation_AndStationLines_WhenLinesProvided()
    {
        var addDto = new AddStation
        {
            SchemaId = 1,
            Name = "Station1",
            LineIds = new List<int> { 10, 20 }
        };

        StationDb savedStation = null!;
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<StationDb>()))
            .Callback<StationDb>(s =>
            {
                s.Id = 100; // Symulacja ustawienia Id po zapisie
                savedStation = s;
            })
            .Returns(Task.CompletedTask);

        _mockStationLineRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _service.AddStationAsync(addDto);

        Assert.Equal("Station1", savedStation.Name);
        Assert.Equal(2, savedStation.StationLines.Count);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<StationDb>()), Times.Once);
        _mockStationLineRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetStationsBySchemaIdAsync_ReturnsMappedDtos()
    {
        var stations = new List<StationDb>
        {
            new StationDb { Id = 1, SchemaId = 10, Name = "S1" },
            new StationDb { Id = 2, SchemaId = 10, Name = "S2" }
        };
        _mockRepo.Setup(r => r.GetAllBySchemaIdAsync(10)).ReturnsAsync(stations);

        var result = await _service.GetStationsBySchemaIdAsync(10);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.Name == "S1");
        Assert.Contains(result, s => s.Name == "S2");
    }

    [Fact]
    public async Task UpdateStationAsync_UpdatesStationProperties_WhenStationExists()
    {
        var station = new StationDb { Id = 1, Name = "OldName" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(station);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<StationDb>())).Returns(Task.CompletedTask);

        var editDto = new EditStation
        {
            Name = "NewName",
            Latitude = (decimal)50.0,
            Longitude = (decimal)20.0
        };

        await _service.UpdateStationAsync(1, editDto);

        Assert.Equal("NewName", station.Name);
        Assert.Equal((decimal)50.0, station.Latitude);
        Assert.Equal((decimal)20.0, station.Longitude);
        _mockRepo.Verify(r => r.UpdateAsync(station), Times.Once);
    }

    [Fact]
    public async Task UpdateStationAsync_ThrowsKeyNotFoundException_WhenStationNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((StationDb)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.UpdateStationAsync(1, new EditStation { Name = "Test" }));
    }

    [Fact]
    public async Task DeleteStationAsync_CallsRepositoryDelete()
    {
        _mockRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        await _service.DeleteStationAsync(1);

        _mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}
