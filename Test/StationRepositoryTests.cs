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

public class StationRepositoryTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"StationDb_{Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddAsync_AddsStation()
    {
        var context = GetInMemoryDbContext();
        var repo = new StationRepository(context);

        var station = new StationDb { Name = "Station1", SchemaId = 1, Description = "" };

        await repo.AddAsync(station);

        var all = await context.Stations.ToListAsync();
        Assert.Single(all);
        Assert.Equal("Station1", all.First().Name);
    }

    [Fact]
    public async Task GetAllBySchemaIdAsync_ReturnsStationsOrderedByName()
    {
        var context = GetInMemoryDbContext();
        await context.Stations.AddRangeAsync(new List<StationDb>
        {
            new StationDb { Name = "BStation", SchemaId = 1, Description = "" },
            new StationDb { Name = "AStation", SchemaId = 1, Description = "" },
            new StationDb { Name = "OtherSchema", SchemaId = 2, Description = "" }
        });
        await context.SaveChangesAsync();

        var repo = new StationRepository(context);

        var result = await repo.GetAllBySchemaIdAsync(1);

        Assert.Equal(2, result.Count);
        Assert.Equal("AStation", result[0].Name);
        Assert.Equal("BStation", result[1].Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsStation_WhenExists()
    {
        var context = GetInMemoryDbContext();
        var station = new StationDb { Name = "Station1", Description = "" };
        await context.Stations.AddAsync(station);
        await context.SaveChangesAsync();

        var repo = new StationRepository(context);

        var result = await repo.GetByIdAsync(station.Id);

        Assert.NotNull(result);
        Assert.Equal("Station1", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        var context = GetInMemoryDbContext();
        var repo = new StationRepository(context);

        var result = await repo.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesStation()
    {
        var context = GetInMemoryDbContext();
        var station = new StationDb { Name = "OldName", Description = "" };
        await context.Stations.AddAsync(station);
        await context.SaveChangesAsync();

        var repo = new StationRepository(context);
        station.Name = "NewName";

        await repo.UpdateAsync(station);

        var updated = await context.Stations.FindAsync(station.Id);
        Assert.Equal("NewName", updated.Name);
    }

    [Fact]
    public async Task DeleteAsync_RemovesStation_WhenExists()
    {
        var context = GetInMemoryDbContext();
        var station = new StationDb { Name = "Station1", Description = "" };
        await context.Stations.AddAsync(station);
        await context.SaveChangesAsync();

        var repo = new StationRepository(context);

        await repo.DeleteAsync(station.Id);

        var deleted = await context.Stations.FindAsync(station.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteAsync_NonExisting_DoesNothing()
    {
        var context = GetInMemoryDbContext();
        var repo = new StationRepository(context);

        await repo.DeleteAsync(999);

        var all = await context.Stations.ToListAsync();
        Assert.Empty(all);
    }
}
