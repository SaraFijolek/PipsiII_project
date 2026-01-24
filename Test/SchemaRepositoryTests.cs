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

public class SchemaRepositoryTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"SchemasDb_{Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddAsync_AddsSchemaToDatabase()
    {
        var context = GetInMemoryDbContext();
        var repo = new SchemaRepository(context);

        var schema = new SchamaDb { Name = "Test Schema", City = "Abc", Country = "PL", Description = "", OwnerId = "1" };

        await repo.AddAsync(schema);

        var all = await context.Schamas.ToListAsync();
        Assert.Single(all);
        Assert.Equal("Test Schema", all.First().Name);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllSchemas_ExcludingDeleted()
    {
        var context = GetInMemoryDbContext();
        await context.Schamas.AddRangeAsync(new List<SchamaDb>
        {
            new SchamaDb { Name = "Schema1", DeletedAt = null, City = "Abc", Country = "PL", Description = "", OwnerId = "1" },
            new SchamaDb { Name = "Schema2", DeletedAt = DateTime.UtcNow, City = "Abc", Country = "PL", Description = "", OwnerId = "1" }
        });
        await context.SaveChangesAsync();

        var repo = new SchemaRepository(context);

        var result = await repo.GetAllAsync(false);

        Assert.Single(result);
        Assert.Equal("Schema1", result.First().Name);
    }

    [Fact]
    public async Task GetAllAsync_IncludeDeleted_ReturnsAll()
    {
        var context = GetInMemoryDbContext();
        await context.Schamas.AddRangeAsync(new List<SchamaDb>
        {
            new SchamaDb { Name = "Schema1", DeletedAt = null, City = "Abc", Country = "PL", Description = "", OwnerId = "1" },
            new SchamaDb { Name = "Schema2", DeletedAt = DateTime.UtcNow, City = "Abc", Country = "PL", Description = "", OwnerId = "1" }
        });
        await context.SaveChangesAsync();

        var repo = new SchemaRepository(context);

        var result = await repo.GetAllAsync(includeDeleted: true);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsSchema_WhenExists()
    {
        var context = GetInMemoryDbContext();
        var schema = new SchamaDb { Name = "Schema1", City = "Abc", Country = "PL", Description = "", OwnerId = "1" };
        await context.Schamas.AddAsync(schema);
        await context.SaveChangesAsync();

        var repo = new SchemaRepository(context);

        var result = await repo.GetByIdAsync(schema.Id);

        Assert.NotNull(result);
        Assert.Equal("Schema1", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        var context = GetInMemoryDbContext();
        var repo = new SchemaRepository(context);

        var result = await repo.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesSchema()
    {
        var context = GetInMemoryDbContext();
        var schema = new SchamaDb { Name = "OldName", City = "Abc", Country = "PL", Description = "", OwnerId = "1" };
        await context.Schamas.AddAsync(schema);
        await context.SaveChangesAsync();

        var repo = new SchemaRepository(context);

        schema.Name = "NewName";
        await repo.UpdateAsync(schema);

        var updated = await context.Schamas.FindAsync(schema.Id);
        Assert.Equal("NewName", updated.Name);
    }
}

