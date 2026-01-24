using Moq;
using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using Schematics.API.DTOs.Schemas;
using Schematics.API.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

public class SchemaServiceTests
{
    [Fact]
    public async Task AddSchemaAsync_CallsRepositoryAdd()
    {
        var mockRepo = new Mock<ISchamaRepository>();
        var service = new SchemaService(mockRepo.Object);
        var dto = new AddSchemaDto
        {
            Name = "Schema1",
            Description = "Desc",
            City = "City",
            Country = "Country",
            IsPublic = true
        };
        var ownerId = "user123";

        await service.AddSchemaAsync(dto, ownerId);

        mockRepo.Verify(r => r.AddAsync(It.Is<SchamaDb>(s =>
            s.Name == dto.Name &&
            s.Description == dto.Description &&
            s.City == dto.City &&
            s.Country == dto.Country &&
            s.IsPublic == dto.IsPublic &&
            s.OwnerId == ownerId
        )), Times.Once);
    }

    [Fact]
    public async Task GetAllSchemasAsync_ReturnsMappedSchemas()
    {
        var mockRepo = new Mock<ISchamaRepository>();
        mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<SchamaDb>
        {
            new SchamaDb { Id = 1, Name = "S1", Description="D1", City="C1", Country="CT1", IsPublic=true },
            new SchamaDb { Id = 2, Name = "S2", Description="D2", City="C2", Country="CT2", IsPublic=false }
        });

        var service = new SchemaService(mockRepo.Object);

        var result = await service.GetAllSchemasAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("S1", result[0].Name);
        Assert.True(result[0].isPublic);
        Assert.Equal("S2", result[1].Name);
        Assert.False(result[1].isPublic);
    }

    [Fact]
    public async Task GetSchemaByIdAsync_ReturnsSchema_WhenExists()
    {
        var mockRepo = new Mock<ISchamaRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new SchamaDb
        {
            Id = 1,
            Name = "Schema1",
            Description = "Desc",
            City = "City",
            Country = "Country",
            IsPublic = true
        });

        var service = new SchemaService(mockRepo.Object);

        var result = await service.GetSchemaByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Schema1", result.Name);
        Assert.True(result.isPublic);
    }

    [Fact]
    public async Task GetSchemaByIdAsync_ReturnsNull_WhenNotExists()
    {
        var mockRepo = new Mock<ISchamaRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((SchamaDb)null);

        var service = new SchemaService(mockRepo.Object);

        var result = await service.GetSchemaByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateSchemaAsync_ReturnsTrue_WhenSchemaExists()
    {
        var existing = new SchamaDb { Id = 1, Name = "Old" };
        var mockRepo = new Mock<ISchamaRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        var service = new SchemaService(mockRepo.Object);

        var dto = new AddSchemaDto
        {
            Name = "New",
            Description = "Desc",
            City = "City",
            Country = "Country",
            IsPublic = true
        };

        var result = await service.UpdateSchemaAsync(1, dto);

        Assert.True(result);
        Assert.Equal("New", existing.Name);
        mockRepo.Verify(r => r.UpdateAsync(existing), Times.Once);
    }

    [Fact]
    public async Task UpdateSchemaAsync_ReturnsFalse_WhenSchemaNotExists()
    {
        var mockRepo = new Mock<ISchamaRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((SchamaDb)null);

        var service = new SchemaService(mockRepo.Object);

        var dto = new AddSchemaDto { Name = "New" };

        var result = await service.UpdateSchemaAsync(999, dto);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteSchemaAsync_ReturnsTrue_WhenSchemaExists()
    {
        var existing = new SchamaDb { Id = 1, Name = "Schema" };
        var mockRepo = new Mock<ISchamaRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        var service = new SchemaService(mockRepo.Object);

        var result = await service.DeleteSchemaAsync(1);

        Assert.True(result);
        Assert.NotNull(existing.DeletedAt);
        mockRepo.Verify(r => r.UpdateAsync(existing), Times.Once);
    }

    [Fact]
    public async Task DeleteSchemaAsync_ReturnsFalse_WhenSchemaNotExists()
    {
        var mockRepo = new Mock<ISchamaRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((SchamaDb)null);

        var service = new SchemaService(mockRepo.Object);

        var result = await service.DeleteSchemaAsync(999);

        Assert.False(result);
    }
}

