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

public class StationLineRepositoryTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"StationLineDb_{Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddAsync_AddsStationLine()
    {
        var context = GetInMemoryDbContext();
        var repo = new StationLineRepository(context);

        var sl = new StationLineDb { StationId = 1, LineId = 2 };

        await repo.AddAsync(sl);
        await repo.SaveChangesAsync();

        var all = await context.StationLines.ToListAsync();
        Assert.Single(all);
        Assert.Equal(1, all.First().StationId);
        Assert.Equal(2, all.First().LineId);
    }

    [Fact]
    public async Task GetByLineIdAsync_ReturnsOnlyMatchingLine()
    {
        var context = GetInMemoryDbContext();
        await context.StationLines.AddRangeAsync(new List<StationLineDb>
        {
            new StationLineDb { StationId = 1, LineId = 10, Station = new StationDb { Name = "A", Description = "" }, Line = new LineDb { Color = "Red", Name = "Abc" } },
            new StationLineDb { StationId = 2, LineId = 10, Station = new StationDb { Name = "A", Description = "" }, Line = new LineDb { Color = "Red", Name = "Abc" } },
            new StationLineDb { StationId = 3, LineId = 20, Station = new StationDb { Name = "A", Description = "" }, Line = new LineDb { Color = "Red", Name = "Abc" } }
        });
        await context.SaveChangesAsync();

        var repo = new StationLineRepository(context);

        var result = await repo.GetByLineIdAsync(10);

        Assert.Equal(2, result.Count);
        Assert.All(result, sl => Assert.Equal(10, sl.LineId));
        Assert.DoesNotContain(result, sl => sl.LineId == 20);
    }

    [Fact]
    public async Task RemoveAsync_RemovesStationLine_WhenExists()
    {
        var context = GetInMemoryDbContext();
        var sl = new StationLineDb { StationId = 1, LineId = 2 };
        await context.StationLines.AddAsync(sl);
        await context.SaveChangesAsync();

        var repo = new StationLineRepository(context);

        await repo.RemoveAsync(1, 2);
        await repo.SaveChangesAsync();

        var deleted = await context.StationLines.FindAsync(1, 2);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task RemoveAsync_NonExisting_DoesNothing()
    {
        var context = GetInMemoryDbContext();
        var repo = new StationLineRepository(context);

        await repo.RemoveAsync(999, 999);
        await repo.SaveChangesAsync();

        var all = await context.StationLines.ToListAsync();
        Assert.Empty(all);
    }
}