using Microsoft.EntityFrameworkCore;
using Schematics.API.Data;
using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

public class LineCategoryRepositoryTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"LineCategoriesDb_{System.Guid.NewGuid()}")
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddAsync_AddsCategoryToDatabase()
    {
        var context = GetInMemoryDbContext();
        var repo = new LineCategoryRepository(context);

        var category = new LineCategoryDb { Name = "Category 1", Color = "Red" };

        await repo.AddAsync(category);

        var categoriesInDb = await context.LineCategories.ToListAsync();
        Assert.Single(categoriesInDb);
        Assert.Equal("Category 1", categoriesInDb.First().Name);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategories()
    {
        var context = GetInMemoryDbContext();
        await context.LineCategories.AddRangeAsync(new List<LineCategoryDb>
        {
            new LineCategoryDb { Name = "Cat1", Color = "Red" },
            new LineCategoryDb { Name = "Cat2", Color = "Blue" }
        });
        await context.SaveChangesAsync();

        var repo = new LineCategoryRepository(context);

        var result = await repo.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Name == "Cat1");
        Assert.Contains(result, c => c.Name == "Cat2");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCategoryWithLines()
    {
        var context = GetInMemoryDbContext();
        var category = new LineCategoryDb
        {
            Name = "Cat1",
            Color = "Green",
            Lines = new List<LineDb>
            {
                new LineDb { Name = "Line1", Color = "Red" },
                new LineDb { Name = "Line2", Color = "Blue" }
            }
        };

        await context.LineCategories.AddAsync(category);
        await context.SaveChangesAsync();

        var repo = new LineCategoryRepository(context);

        var result = await repo.GetByIdAsync(category.Id);

        Assert.NotNull(result);
        Assert.Equal("Cat1", result.Name);
        Assert.Equal(2, result.Lines.Count);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesCategory()
    {
        var context = GetInMemoryDbContext();
        var category = new LineCategoryDb { Name = "OldName", Color = "Red" };
        await context.LineCategories.AddAsync(category);
        await context.SaveChangesAsync();

        var repo = new LineCategoryRepository(context);
        category.Name = "NewName";

        await repo.UpdateAsync(category);

        var updated = await context.LineCategories.FindAsync(category.Id);
        Assert.Equal("NewName", updated.Name);
    }

    [Fact]
    public async Task DeleteAsync_RemovesCategory()
    {
        var context = GetInMemoryDbContext();
        var category = new LineCategoryDb { Name = "ToDelete", Color = "Red" };
        await context.LineCategories.AddAsync(category);
        await context.SaveChangesAsync();

        var repo = new LineCategoryRepository(context);

        await repo.DeleteAsync(category.Id);

        var deleted = await context.LineCategories.FindAsync(category.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_DoesNothing()
    {
        var context = GetInMemoryDbContext();
        var repo = new LineCategoryRepository(context);

        await repo.DeleteAsync(999);
    }
}

