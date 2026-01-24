using Microsoft.EntityFrameworkCore;
using Schematics.API.Data;
using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

public class LineRepositoryTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"LinesDb_{System.Guid.NewGuid()}")
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddAsync_AddsLineToDatabase()
    {
        var context = GetInMemoryDbContext();
        var repo = new LineRepository(context);

        var line = new LineDb { Name = "Line 1", SchemaId = 1, Color = "Red" };

        await repo.AddAsync(line);

        var linesInDb = await context.Lines.ToListAsync();
        Assert.Single(linesInDb);
        Assert.Equal("Line 1", linesInDb.First().Name);
        Assert.Equal(1, linesInDb.First().SchemaId);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllLines()
    {
        var context = GetInMemoryDbContext();
        await context.Lines.AddRangeAsync(new List<LineDb>
        {
            new LineDb { Name = "Line1", SchemaId = 1, Color = "Red" },
            new LineDb { Name = "Line2", SchemaId = 2 , Color = "Blue"}
        });
        await context.SaveChangesAsync();

        var repo = new LineRepository(context);

        var result = await repo.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, l => l.Name == "Line1");
        Assert.Contains(result, l => l.Name == "Line2");
    }

    [Fact]
    public async Task GetBySchemaIdAsync_ReturnsOnlyMatchingLines()
    {
        var context = GetInMemoryDbContext();
        await context.Lines.AddRangeAsync(new List<LineDb>
        {
            new LineDb { Name = "Line1", SchemaId = 1, Color = "Red" },
            new LineDb { Name = "Line2", SchemaId = 1 , Color = "Blue"},
            new LineDb { Name = "Line3", SchemaId = 2 , Color = "Green"}
        });
        await context.SaveChangesAsync();

        var repo = new LineRepository(context);

        var result = await repo.GetBySchemaIdAsync(1);

        Assert.Equal(2, result.Count);
        Assert.All(result, l => Assert.Equal(1, l.SchemaId));
    }

    [Fact]
    public async Task GetBySchemaIdAsync_NoMatches_ReturnsEmptyList()
    {
        var context = GetInMemoryDbContext();
        var repo = new LineRepository(context);

        var result = await repo.GetBySchemaIdAsync(999);

        Assert.Empty(result);
    }
}

