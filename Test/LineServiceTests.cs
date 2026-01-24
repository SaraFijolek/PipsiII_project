using Moq;
using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using Schematics.API.DTOs.Lines;
using Schematics.API.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

public class LineServiceTests
{
    private readonly Mock<ILineRepository> _lineRepoMock;
    private readonly Mock<ILineCategoryRepository> _categoryRepoMock;
    private readonly LineService _service;

    public LineServiceTests()
    {
        _lineRepoMock = new Mock<ILineRepository>();
        _categoryRepoMock = new Mock<ILineCategoryRepository>();
        _service = new LineService(_lineRepoMock.Object, _categoryRepoMock.Object);
    }

    [Fact]
    public async Task AddLineAsync_CategoryExists_AddsLine()
    {
        var model = new LineDto
        {
            SchemaId = 1,
            LineCategoryId = 2,
            Name = "Line1",
            LineNumber = 10,
            Color = "Red",
            IsCircular = false
        };

        _categoryRepoMock.Setup(c => c.GetByIdAsync(2))
                         .ReturnsAsync(new LineCategoryDb { Id = 2, Name = "Cat1" });

        await _service.AddLineAsync(model);

        _lineRepoMock.Verify(r => r.AddAsync(It.Is<LineDb>(
            l => l.SchemaId == 1 &&
                 l.LineCategoryId == 2 &&
                 l.Name == "Line1" &&
                 l.LineNumber == 10 &&
                 l.Color == "Red" &&
                 l.IsCircular == false
        )), Times.Once);
    }

    [Fact]
    public async Task AddLineAsync_CategoryDoesNotExist_ThrowsKeyNotFoundException()
    {
        var model = new LineDto { LineCategoryId = 99 };

        _categoryRepoMock.Setup(c => c.GetByIdAsync(99)).ReturnsAsync((LineCategoryDb)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.AddLineAsync(model));

        _lineRepoMock.Verify(r => r.AddAsync(It.IsAny<LineDb>()), Times.Never);
    }

    [Fact]
    public async Task GetAllLinesAsync_ReturnsMappedLines()
    {
        var lines = new List<LineDb>
        {
            new LineDb { Id = 1, Name = "Line1", SchemaId = 1, LineCategoryId = 1, LineNumber = 10, Color = "Red", IsCircular = false },
            new LineDb { Id = 2, Name = "Line2", SchemaId = 2, LineCategoryId = 2, LineNumber = 20, Color = "Blue", IsCircular = true }
        };

        _lineRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(lines);

        var result = await _service.GetAllLinesAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, l => l.Name == "Line1" && l.IsCircular == false);
        Assert.Contains(result, l => l.Name == "Line2" && l.IsCircular == true);
    }

    [Fact]
    public async Task GetLinesBySchemaIdAsync_FiltersCorrectly()
    {
        var lines = new List<LineDb>
        {
            new LineDb { Id = 1, Name = "Line1", SchemaId = 1 },
            new LineDb { Id = 2, Name = "Line2", SchemaId = 1 },
            new LineDb { Id = 3, Name = "Line3", SchemaId = 2 }
        };

        _lineRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(lines);

        var result = await _service.GetLinesBySchemaIdAsync(1);

        Assert.Equal(2, result.Count);
        Assert.All(result, l => Assert.Equal(1, l.SchemaId));
        Assert.DoesNotContain(result, l => l.SchemaId == 2);
    }

    [Fact]
    public async Task GetLinesBySchemaIdAsync_NoMatches_ReturnsEmptyList()
    {
        _lineRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<LineDb>());

        var result = await _service.GetLinesBySchemaIdAsync(999);

        Assert.Empty(result);
    }
}

