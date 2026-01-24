using Microsoft.AspNetCore.Mvc;
using Moq;
using Schematics.API.Controllers;
using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using Schematics.API.DTOs.LineCategories;
using Xunit;
using Assert = Xunit.Assert;

public class LineCategoryControllerTests
{
    private readonly Mock<ILineCategoryRepository> _repositoryMock;
    private readonly LineCategoryController _controller;

    public LineCategoryControllerTests()
    {
        _repositoryMock = new Mock<ILineCategoryRepository>();
        _controller = new LineCategoryController(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithDtoList()
    {
        var categories = new List<LineCategoryDb>
        {
            new LineCategoryDb { Id = 1, Name = "A", Color = "Red" },
            new LineCategoryDb { Id = 2, Name = "B", Color = "Blue" }
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(categories);

        var result = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<List<LineCategoryDto>>(ok.Value);
        Assert.Equal(2, dto.Count);
    }

    [Fact]
    public async Task GetById_CategoryNotFound_ReturnsNotFound()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((LineCategoryDb)null);

        var result = await _controller.GetById(1);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetById_CategoryExists_ReturnsOk()
    {
        var category = new LineCategoryDb
        {
            Id = 1,
            Name = "Test",
            Color = "Green"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(category);

        var result = await _controller.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<LineCategoryDto>(ok.Value);
        Assert.Equal("Test", dto.Name);
    }

    [Fact]
    public async Task Create_ValidDto_ReturnsCreatedAtAction()
    {
        var dto = new AddLinecategory
        {
            Name = "New",
            Color = "Yellow"
        };

        var result = await _controller.Create(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(LineCategoryController.GetById), created.ActionName);

        _repositoryMock.Verify(
            r => r.AddAsync(It.IsAny<LineCategoryDb>()),
            Times.Once);
    }

    [Fact]
    public async Task Update_CategoryNotFound_ReturnsNotFound()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((LineCategoryDb)null);

        var result = await _controller.Update(1, new EditLineCategory());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Update_CategoryExists_ReturnsNoContent()
    {
        var category = new LineCategoryDb { Id = 1 };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(category);

        var dto = new EditLineCategory
        {
            Name = "Updated",
            Color = "Black"
        };

        var result = await _controller.Update(1, dto);

        Assert.IsType<NoContentResult>(result);

        _repositoryMock.Verify(
            r => r.UpdateAsync(It.Is<LineCategoryDb>(
                c => c.Name == "Updated" && c.Color == "Black")),
            Times.Once);
    }

    [Fact]
    public async Task Delete_ValidId_ReturnsNoContent()
    {
        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);

        _repositoryMock.Verify(
            r => r.DeleteAsync(1),
            Times.Once);
    }
}
