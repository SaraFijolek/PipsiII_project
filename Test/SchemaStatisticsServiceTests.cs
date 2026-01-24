using Moq;
using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using Schematics.API.DTOs.SchemaStatistics;
using Schematics.API.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

public class SchemaStatisticsServiceTests
{
    [Fact]
    public async Task GetStatisticsBySchemaIdAsync_ReturnsDto_WhenExists()
    {
        var schema = new SchamaDb { Id = 1, Name = "Schema1" };
        var stats = new SchemaStatisticsDb
        {
            Id = 10,
            SchemaId = 1,
            Schema = schema,
            StationCount = 5,
            LineCount = 2
        };

        var mockStatsRepo = new Mock<ISchemaStatisticsRepository>();
        mockStatsRepo.Setup(r => r.GetBySchemaIdAsync(1)).ReturnsAsync(stats);

        var mockSchemaRepo = new Mock<ISchamaRepository>();

        var service = new SchemaStatisticsService(mockStatsRepo.Object, mockSchemaRepo.Object);

        var result = await service.GetStatisticsBySchemaIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("Schema1", result.SchemaName);
        Assert.Equal(5, result.StationCount);
        Assert.Equal(2, result.LineCount);
    }

    [Fact]
    public async Task GetStatisticsBySchemaIdAsync_ReturnsNull_WhenNotExists()
    {
        var mockStatsRepo = new Mock<ISchemaStatisticsRepository>();
        mockStatsRepo.Setup(r => r.GetBySchemaIdAsync(1)).ReturnsAsync((SchemaStatisticsDb)null);

        var mockSchemaRepo = new Mock<ISchamaRepository>();
        var service = new SchemaStatisticsService(mockStatsRepo.Object, mockSchemaRepo.Object);

        var result = await service.GetStatisticsBySchemaIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllStatisticsAsync_ReturnsMappedDtos()
    {
        var schema = new SchamaDb { Id = 1, Name = "S1" };
        var statsList = new List<SchemaStatisticsDb>
        {
            new SchemaStatisticsDb { Id = 10, SchemaId = 1, Schema = schema, StationCount = 5 },
            new SchemaStatisticsDb { Id = 20, SchemaId = 2, Schema = schema, StationCount = 3 }
        };

        var mockStatsRepo = new Mock<ISchemaStatisticsRepository>();
        mockStatsRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(statsList);

        var mockSchemaRepo = new Mock<ISchamaRepository>();
        var service = new SchemaStatisticsService(mockStatsRepo.Object, mockSchemaRepo.Object);

        var result = await service.GetAllStatisticsAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal(10, result[0].Id);
        Assert.Equal(20, result[1].Id);
    }

    [Fact]
    public async Task UpdateStatisticsAsync_AddsNew_WhenNotExists()
    {
        var mockStatsRepo = new Mock<ISchemaStatisticsRepository>();
        mockStatsRepo.Setup(r => r.GetBySchemaIdAsync(1)).ReturnsAsync((SchemaStatisticsDb)null);

        var mockSchemaRepo = new Mock<ISchamaRepository>();
        var service = new SchemaStatisticsService(mockStatsRepo.Object, mockSchemaRepo.Object);

        var dto = new UpdateStatisticsDto { StationCount = 5, LineCount = 2, ConnectionCount = 1, TotalNetworkLength = 100 };

        await service.UpdateStatisticsAsync(1, dto);

        mockStatsRepo.Verify(r => r.AddAsync(It.Is<SchemaStatisticsDb>(
            s => s.SchemaId == 1 && s.StationCount == 5 && s.LineCount == 2 && s.ConnectionCount == 1 && s.TotalNetworkLength == 100
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateStatisticsAsync_UpdatesExisting_WhenExists()
    {
        var existing = new SchemaStatisticsDb { Id = 10, SchemaId = 1, StationCount = 0 };
        var mockStatsRepo = new Mock<ISchemaStatisticsRepository>();
        mockStatsRepo.Setup(r => r.GetBySchemaIdAsync(1)).ReturnsAsync(existing);

        var mockSchemaRepo = new Mock<ISchamaRepository>();
        var service = new SchemaStatisticsService(mockStatsRepo.Object, mockSchemaRepo.Object);

        var dto = new UpdateStatisticsDto { StationCount = 5, LineCount = 2, ConnectionCount = 1, TotalNetworkLength = 100 };

        await service.UpdateStatisticsAsync(1, dto);

        Assert.Equal(5, existing.StationCount);
        Assert.Equal(2, existing.LineCount);
        Assert.Equal(1, existing.ConnectionCount);
        Assert.Equal(100, existing.TotalNetworkLength);

        mockStatsRepo.Verify(r => r.UpdateAsync(existing), Times.Once);
    }

    [Fact]
    public async Task RecalculateStatisticsAsync_CreatesOrUpdatesStatistics()
    {
        var schema = new SchamaDb { Id = 1 };
        var mockSchemaRepo = new Mock<ISchamaRepository>();
        mockSchemaRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(schema);

        var mockStatsRepo = new Mock<ISchemaStatisticsRepository>();
        mockStatsRepo.Setup(r => r.GetBySchemaIdAsync(1)).ReturnsAsync((SchemaStatisticsDb)null);

        var service = new SchemaStatisticsService(mockStatsRepo.Object, mockSchemaRepo.Object);

        await service.RecalculateStatisticsAsync(1);

        mockStatsRepo.Verify(r => r.AddAsync(It.Is<SchemaStatisticsDb>(s => s.SchemaId == 1 && s.StationCount == 0 && s.LineCount == 0)), Times.Once);
    }

    [Fact]
    public async Task RecalculateStatisticsAsync_Throws_WhenSchemaNotFound()
    {
        var mockSchemaRepo = new Mock<ISchamaRepository>();
        mockSchemaRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((SchamaDb)null);

        var mockStatsRepo = new Mock<ISchemaStatisticsRepository>();
        var service = new SchemaStatisticsService(mockStatsRepo.Object, mockSchemaRepo.Object);

        await Assert.ThrowsAsync<Exception>(() => service.RecalculateStatisticsAsync(1));
    }
}

