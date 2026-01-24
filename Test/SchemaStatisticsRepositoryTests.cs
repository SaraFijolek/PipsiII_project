using Microsoft.EntityFrameworkCore;
using Schematics.API.Data;
using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

public class SchemaStatisticsRepositoryTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"SchemaStatsDb_{Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddAsync_AddsStatisticsToDatabase()
    {
        var context = GetInMemoryDbContext();
        var repo = new SchemaStatisticsRepository(context);

        var stats = new SchemaStatisticsDb { SchemaId = 1, LineCount = 5 };

        await repo.AddAsync(stats);

        var all = await context.SchemaStatistics.ToListAsync();
        Assert.Single(all);
        Assert.Equal(1, all.First().SchemaId);
        Assert.Equal(5, all.First().LineCount);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllStatistics()
    {
        var context = GetInMemoryDbContext();
        await context.SchemaStatistics.AddRangeAsync(new List<SchemaStatisticsDb>
        {
            new SchemaStatisticsDb { SchemaId = 1, Schema = new SchamaDb { Name = "A", City = "Abc", Country = "PL", Description = "", OwnerId = "1" } },
            new SchemaStatisticsDb { SchemaId = 2, Schema = new SchamaDb { Name = "A", City = "Abc", Country = "PL", Description = "", OwnerId = "1" } }
        });
        await context.SaveChangesAsync();

        var repo = new SchemaStatisticsRepository(context);

        var result = await repo.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.SchemaId == 1);
        Assert.Contains(result, s => s.SchemaId == 2);
    }

    [Fact]
    public async Task GetBySchemaIdAsync_ReturnsStatistics_WhenExists()
    {
        var context = GetInMemoryDbContext();
        var stats = new SchemaStatisticsDb { SchemaId = 1, LineCount = 5, Schema = new SchamaDb { Name = "A", City = "Abc", Country = "PL", Description = "", OwnerId = "1" } };
        await context.SchemaStatistics.AddAsync(stats);
        await context.SaveChangesAsync();

        var repo = new SchemaStatisticsRepository(context);

        var result = await repo.GetBySchemaIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.SchemaId);
        Assert.Equal(5, result.LineCount);
    }

    [Fact]
    public async Task GetBySchemaIdAsync_ReturnsNull_WhenNotExists()
    {
        var context = GetInMemoryDbContext();
        var repo = new SchemaStatisticsRepository(context);

        var result = await repo.GetBySchemaIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesStatisticsAndSetsUpdatedAt()
    {
        var context = GetInMemoryDbContext();
        var stats = new SchemaStatisticsDb { SchemaId = 1, LineCount = 5 };
        await context.SchemaStatistics.AddAsync(stats);
        await context.SaveChangesAsync();

        var repo = new SchemaStatisticsRepository(context);

        stats.LineCount = 10;
        await repo.UpdateAsync(stats);

        var updated = await context.SchemaStatistics.FindAsync(stats.Id);
        Assert.Equal(10, updated.LineCount);
        Assert.NotNull(updated.UpdatedAt);
        Assert.True(updated.UpdatedAt > DateTime.UtcNow.AddSeconds(-5));
    }

    [Fact]
    public async Task DeleteBySchemaIdAsync_RemovesStatistics_WhenExists()
    {
        var context = GetInMemoryDbContext();
        var stats = new SchemaStatisticsDb { SchemaId = 1, Schema = new SchamaDb { Name = "A", City = "Abc", Country = "PL", Description = "", OwnerId = "1" } };
        await context.SchemaStatistics.AddAsync(stats);
        await context.SaveChangesAsync();

        var repo = new SchemaStatisticsRepository(context);

        await repo.DeleteBySchemaIdAsync(1);

        var deleted = await context.SchemaStatistics.FindAsync(stats.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteBySchemaIdAsync_NonExistingId_DoesNothing()
    {
        var context = GetInMemoryDbContext();
        var repo = new SchemaStatisticsRepository(context);

        await repo.DeleteBySchemaIdAsync(999);

        var all = await context.SchemaStatistics.ToListAsync();
        Assert.Empty(all);
    }
}

